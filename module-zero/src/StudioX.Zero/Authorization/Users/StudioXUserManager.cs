using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StudioX.Application.Features;
using StudioX.Authorization.Roles;
using StudioX.Configuration;
using StudioX.Domain.Repositories;
using StudioX.Domain.Services;
using StudioX.Domain.Uow;
using StudioX.IdentityFramework;
using StudioX.Localization;
using StudioX.MultiTenancy;
using StudioX.Organizations;
using StudioX.Runtime.Caching;
using StudioX.Runtime.Security;
using StudioX.Runtime.Session;
using StudioX.Zero;
using StudioX.Zero.Configuration;
using Microsoft.AspNet.Identity;

namespace StudioX.Authorization.Users
{
    /// <summary>
    /// Extends <see cref="UserManager{TUser,TKey}"/> of ASP.NET Identity Framework.
    /// </summary>
    public abstract class StudioXUserManager<TRole, TUser>
        : UserManager<TUser, long>,
        IDomainService
        where TRole : StudioXRole<TUser>, new()
        where TUser : StudioXUser<TUser>
    {
        protected IUserPermissionStore<TUser> UserPermissionStore
        {
            get
            {
                if (!(Store is IUserPermissionStore<TUser>))
                {
                    throw new StudioXException("Store is not IUserPermissionStore");
                }

                return Store as IUserPermissionStore<TUser>;
            }
        }

        public ILocalizationManager LocalizationManager { get; }

        public IStudioXSession StudioXSession { get; set; }

        public FeatureDependencyContext FeatureDependencyContext { get; set; }

        protected StudioXRoleManager<TRole, TUser> RoleManager { get; }

        public StudioXUserStore<TRole, TUser> StudioXStore { get; }

        private readonly IPermissionManager permissionManager;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly ICacheManager cacheManager;
        private readonly IRepository<OrganizationUnit, long> organizationUnitRepository;
        private readonly IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository;
        private readonly IOrganizationUnitSettings organizationUnitSettings;
        private readonly ISettingManager settingManager;

        protected StudioXUserManager(
            StudioXUserStore<TRole, TUser> userStore,
            StudioXRoleManager<TRole, TUser> roleManager,
            IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager,
            ICacheManager cacheManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IOrganizationUnitSettings organizationUnitSettings,
            ILocalizationManager localizationManager,
            IdentityEmailMessageService emailService, 
            ISettingManager settingManager,
            IUserTokenProviderAccessor userTokenProviderAccessor)
            : base(userStore)
        {
            StudioXStore = userStore;
            RoleManager = roleManager;
            LocalizationManager = localizationManager;
            this.settingManager = settingManager;

            this.permissionManager = permissionManager;
            this.unitOfWorkManager = unitOfWorkManager;
            this.cacheManager = cacheManager;
            this.organizationUnitRepository = organizationUnitRepository;
            this.userOrganizationUnitRepository = userOrganizationUnitRepository;
            this.organizationUnitSettings = organizationUnitSettings;

            StudioXSession = NullStudioXSession.Instance;

            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;
            
            EmailService = emailService;

            UserTokenProvider = userTokenProviderAccessor.GetUserTokenProviderOrNull<TUser>();
        }

        public override async Task<IdentityResult> CreateAsync(TUser user)
        {
            var result = await CheckDuplicateUsernameOrEmailAddressAsync(user.Id, user.UserName, user.EmailAddress);
            if (!result.Succeeded)
            {
                return result;
            }

            var tenantId = GetCurrentTenantId();
            if (tenantId.HasValue && !user.TenantId.HasValue)
            {
                user.TenantId = tenantId.Value;
            }

            return await base.CreateAsync(user);
        }

        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="permissionName">Permission name</param>
        public virtual async Task<bool> IsGrantedAsync(long userId, string permissionName)
        {
            return await IsGrantedAsync(
                userId,
                permissionManager.GetPermission(permissionName)
                );
        }

        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        public virtual Task<bool> IsGrantedAsync(TUser user, Permission permission)
        {
            return IsGrantedAsync(user.Id, permission);
        }

        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="permission">Permission</param>
        public virtual async Task<bool> IsGrantedAsync(long userId, Permission permission)
        {
            //Check for multi-tenancy side
            if (!permission.MultiTenancySides.HasFlag(StudioXSession.MultiTenancySide))
            {
                return false;
            }

            //Check for depended features
            if (permission.FeatureDependency != null && StudioXSession.MultiTenancySide == MultiTenancySides.Tenant)
            {
                if (!await permission.FeatureDependency.IsSatisfiedAsync(FeatureDependencyContext))
                {
                    return false;
                }
            }
            
            //Get cached user permissions
            var cacheItem = await GetUserPermissionCacheItemAsync(userId);
            if (cacheItem == null)
            {
                return false;
            }

            //Check for user-specific value
            if (cacheItem.GrantedPermissions.Contains(permission.Name))
            {
                return true;
            }

            if (cacheItem.ProhibitedPermissions.Contains(permission.Name))
            {
                return false;
            }

            //Check for roles
            foreach (var roleId in cacheItem.RoleIds)
            {
                if (await RoleManager.IsGrantedAsync(roleId, permission))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets granted permissions for a user.
        /// </summary>
        /// <param name="user">Role</param>
        /// <returns>List of granted permissions</returns>
        public virtual async Task<IReadOnlyList<Permission>> GetGrantedPermissionsAsync(TUser user)
        {
            var permissionList = new List<Permission>();

            foreach (var permission in permissionManager.GetAllPermissions())
            {
                if (await IsGrantedAsync(user.Id, permission))
                {
                    permissionList.Add(permission);
                }
            }

            return permissionList;
        }

        /// <summary>
        /// Sets all granted permissions of a user at once.
        /// Prohibits all other permissions.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="permissions">Permissions</param>
        public virtual async Task SetGrantedPermissionsAsync(TUser user, IEnumerable<Permission> permissions)
        {
            var oldPermissions = await GetGrantedPermissionsAsync(user);
            var newPermissions = permissions.ToArray();

            foreach (var permission in oldPermissions.Where(p => !newPermissions.Contains(p)))
            {
                await ProhibitPermissionAsync(user, permission);
            }

            foreach (var permission in newPermissions.Where(p => !oldPermissions.Contains(p)))
            {
                await GrantPermissionAsync(user, permission);
            }
        }

        /// <summary>
        /// Prohibits all permissions for a user.
        /// </summary>
        /// <param name="user">User</param>
        public async Task ProhibitAllPermissionsAsync(TUser user)
        {
            foreach (var permission in permissionManager.GetAllPermissions())
            {
                await ProhibitPermissionAsync(user, permission);
            }
        }

        /// <summary>
        /// Resets all permission settings for a user.
        /// It removes all permission settings for the user.
        /// User will have permissions according to his roles.
        /// This method does not prohibit all permissions.
        /// For that, use <see cref="ProhibitAllPermissionsAsync"/>.
        /// </summary>
        /// <param name="user">User</param>
        public async Task ResetAllPermissionsAsync(TUser user)
        {
            await UserPermissionStore.RemoveAllPermissionSettingsAsync(user);
        }

        /// <summary>
        /// Grants a permission for a user if not already granted.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        public virtual async Task GrantPermissionAsync(TUser user, Permission permission)
        {
            await UserPermissionStore.RemovePermissionAsync(user, new PermissionGrantInfo(permission.Name, false));

            if (await IsGrantedAsync(user.Id, permission))
            {
                return;
            }

            await UserPermissionStore.AddPermissionAsync(user, new PermissionGrantInfo(permission.Name, true));
        }

        /// <summary>
        /// Prohibits a permission for a user if it's granted.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="permission">Permission</param>
        public virtual async Task ProhibitPermissionAsync(TUser user, Permission permission)
        {
            await UserPermissionStore.RemovePermissionAsync(user, new PermissionGrantInfo(permission.Name, true));

            if (!await IsGrantedAsync(user.Id, permission))
            {
                return;
            }

            await UserPermissionStore.AddPermissionAsync(user, new PermissionGrantInfo(permission.Name, false));
        }

        public virtual async Task<TUser> FindByNameOrEmailAsync(string userNameOrEmailAddress)
        {
            return await StudioXStore.FindByNameOrEmailAsync(userNameOrEmailAddress);
        }

        public virtual Task<List<TUser>> FindAllAsync(UserLoginInfo login)
        {
            return StudioXStore.FindAllAsync(login);
        }

        /// <summary>
        /// Gets a user by given id.
        /// Throws exception if no user found with given id.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User</returns>
        /// <exception cref="StudioXException">Throws exception if no user found with given id</exception>
        public virtual async Task<TUser> GetUserByIdAsync(long userId)
        {
            var user = await FindByIdAsync(userId);
            if (user == null)
            {
                throw new StudioXException("There is no user with id: " + userId);
            }

            return user;
        }

        public async override Task<ClaimsIdentity> CreateIdentityAsync(TUser user, string authenticationType)
        {
            var identity = await base.CreateIdentityAsync(user, authenticationType);
            if (user.TenantId.HasValue)
            {
                identity.AddClaim(new Claim(StudioXClaimTypes.TenantId, user.TenantId.Value.ToString(CultureInfo.InvariantCulture)));
            }

            return identity;
        }

        public async override Task<IdentityResult> UpdateAsync(TUser user)
        {
            var result = await CheckDuplicateUsernameOrEmailAddressAsync(user.Id, user.UserName, user.EmailAddress);
            if (!result.Succeeded)
            {
                return result;
            }

            //Admin user's username can not be changed!
            if (user.UserName != StudioXUser<TUser>.AdminUserName)
            {
                if ((await GetOldUserNameAsync(user.Id)) == StudioXUser<TUser>.AdminUserName)
                {
                    return StudioXIdentityResult.Failed(string.Format(L("CanNotRenameAdminUser"), StudioXUser<TUser>.AdminUserName));
                }
            }

            return await base.UpdateAsync(user);
        }

        public async override Task<IdentityResult> DeleteAsync(TUser user)
        {
            if (user.UserName == StudioXUser<TUser>.AdminUserName)
            {
                return StudioXIdentityResult.Failed(string.Format(L("CanNotDeleteAdminUser"), StudioXUser<TUser>.AdminUserName));
            }

            return await base.DeleteAsync(user);
        }

        public virtual async Task<IdentityResult> ChangePasswordAsync(TUser user, string newPassword)
        {
            var result = await PasswordValidator.ValidateAsync(newPassword);
            if (!result.Succeeded)
            {
                return result;
            }

            await StudioXStore.SetPasswordHashAsync(user, PasswordHasher.HashPassword(newPassword));
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> CheckDuplicateUsernameOrEmailAddressAsync(long? expectedUserId, string userName, string emailAddress)
        {
            var user = (await FindByNameAsync(userName));
            if (user != null && user.Id != expectedUserId)
            {
                return StudioXIdentityResult.Failed(string.Format(L("Identity.DuplicateUserName"), userName));
            }

            user = (await FindByEmailAsync(emailAddress));
            if (user != null && user.Id != expectedUserId)
            {
                return StudioXIdentityResult.Failed(string.Format(L("Identity.DuplicateEmail"), emailAddress));
            }

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> SetRoles(TUser user, string[] roleNames)
        {
            //Remove from removed roles
            foreach (var userRole in user.Roles.ToList())
            {
                var role = await RoleManager.FindByIdAsync(userRole.RoleId);
                if (roleNames.All(roleName => role.Name != roleName))
                {
                    var result = await RemoveFromRoleAsync(user.Id, role.Name);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }
            }

            //Add to added roles
            foreach (var roleName in roleNames)
            {
                var role = await RoleManager.GetRoleByNameAsync(roleName);
                if (user.Roles.All(ur => ur.RoleId != role.Id))
                {
                    var result = await AddToRoleAsync(user.Id, roleName);
                    if (!result.Succeeded)
                    {
                        return result;
                    }
                }
            }

            return IdentityResult.Success;
        }

        public virtual async Task<bool> IsInOrganizationUnitAsync(long userId, long ouId)
        {
            return await IsInOrganizationUnitAsync(
                await GetUserByIdAsync(userId),
                await organizationUnitRepository.GetAsync(ouId)
                );
        }

        public virtual async Task<bool> IsInOrganizationUnitAsync(TUser user, OrganizationUnit ou)
        {
            return await userOrganizationUnitRepository.CountAsync(uou =>
                uou.UserId == user.Id && uou.OrganizationUnitId == ou.Id
                ) > 0;
        }

        public virtual async Task AddToOrganizationUnitAsync(long userId, long ouId)
        {
            await AddToOrganizationUnitAsync(
                await GetUserByIdAsync(userId),
                await organizationUnitRepository.GetAsync(ouId)
                );
        }

        public virtual async Task AddToOrganizationUnitAsync(TUser user, OrganizationUnit ou)
        {
            var currentOus = await GetOrganizationUnitsAsync(user);

            if (currentOus.Any(cou => cou.Id == ou.Id))
            {
                return;
            }

            await CheckMaxUserOrganizationUnitMembershipCountAsync(user.TenantId, currentOus.Count + 1);

            await userOrganizationUnitRepository.InsertAsync(new UserOrganizationUnit(user.TenantId, user.Id, ou.Id));
        }

        public virtual async Task RemoveFromOrganizationUnitAsync(long userId, long ouId)
        {
            await RemoveFromOrganizationUnitAsync(
                await GetUserByIdAsync(userId),
                await organizationUnitRepository.GetAsync(ouId)
                );
        }

        public virtual async Task RemoveFromOrganizationUnitAsync(TUser user, OrganizationUnit ou)
        {
            await userOrganizationUnitRepository.DeleteAsync(uou => uou.UserId == user.Id && uou.OrganizationUnitId == ou.Id);
        }

        public virtual async Task SetOrganizationUnitsAsync(long userId, params long[] organizationUnitIds)
        {
            await SetOrganizationUnitsAsync(
                await GetUserByIdAsync(userId),
                organizationUnitIds
                );
        }

        private async Task CheckMaxUserOrganizationUnitMembershipCountAsync(int? tenantId, int requestedCount)
        {
            var maxCount = await organizationUnitSettings.GetMaxUserMembershipCountAsync(tenantId);
            if (requestedCount > maxCount)
            {
                throw new StudioXException(string.Format("Can not set more than {0} organization unit for a user!", maxCount));
            }
        }

        public virtual async Task SetOrganizationUnitsAsync(TUser user, params long[] organizationUnitIds)
        {
            if (organizationUnitIds == null)
            {
                organizationUnitIds = new long[0];
            }

            await CheckMaxUserOrganizationUnitMembershipCountAsync(user.TenantId, organizationUnitIds.Length);

            var currentOus = await GetOrganizationUnitsAsync(user);

            //Remove from removed OUs
            foreach (var currentOu in currentOus)
            {
                if (!organizationUnitIds.Contains(currentOu.Id))
                {
                    await RemoveFromOrganizationUnitAsync(user, currentOu);
                }
            }

            //Add to added OUs
            foreach (var organizationUnitId in organizationUnitIds)
            {
                if (currentOus.All(ou => ou.Id != organizationUnitId))
                {
                    await AddToOrganizationUnitAsync(
                        user,
                        await organizationUnitRepository.GetAsync(organizationUnitId)
                        );
                }
            }
        }

        [UnitOfWork]
        public virtual Task<List<OrganizationUnit>> GetOrganizationUnitsAsync(TUser user)
        {
            var query = from uou in userOrganizationUnitRepository.GetAll()
                        join ou in organizationUnitRepository.GetAll() on uou.OrganizationUnitId equals ou.Id
                        where uou.UserId == user.Id
                        select ou;

            return Task.FromResult(query.ToList());
        }

        [UnitOfWork]
        public virtual Task<List<TUser>> GetUsersInOrganizationUnit(OrganizationUnit organizationUnit, bool includeChildren = false)
        {
            if (!includeChildren)
            {
                var query = from uou in userOrganizationUnitRepository.GetAll()
                            join user in StudioXStore.Users on uou.UserId equals user.Id
                            where uou.OrganizationUnitId == organizationUnit.Id
                            select user;

                return Task.FromResult(query.ToList());
            }
            else
            {
                var query = from uou in userOrganizationUnitRepository.GetAll()
                            join user in StudioXStore.Users on uou.UserId equals user.Id
                            join ou in organizationUnitRepository.GetAll() on uou.OrganizationUnitId equals ou.Id
                            where ou.Code.StartsWith(organizationUnit.Code)
                            select user;

                return Task.FromResult(query.ToList());
            }
        }

        public virtual void RegisterTwoFactorProviders(int? tenantId)
        {
            TwoFactorProviders.Clear();

            if (!IsTrue(StudioXZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled, tenantId))
            {
                return;
            }

            if (EmailService != null &&
                IsTrue(StudioXZeroSettingNames.UserManagement.TwoFactorLogin.IsEmailProviderEnabled, tenantId))
            {
                RegisterTwoFactorProvider(
                    L("Email"),
                    new EmailTokenProvider<TUser, long>
                    {
                        Subject = L("EmailSecurityCodeSubject"),
                        BodyFormat = L("EmailSecurityCodeBody")
                    }
                );
            }

            if (SmsService != null &&
                IsTrue(StudioXZeroSettingNames.UserManagement.TwoFactorLogin.IsSmsProviderEnabled, tenantId))
            {
                RegisterTwoFactorProvider(
                    L("Sms"),
                    new PhoneNumberTokenProvider<TUser, long>
                    {
                        MessageFormat = L("SmsSecurityCodeMessage")
                    }
                );
            }
        }

        public virtual void InitializeLockoutSettings(int? tenantId)
        {
            UserLockoutEnabledByDefault = IsTrue(StudioXZeroSettingNames.UserManagement.UserLockOut.IsEnabled, tenantId);
            DefaultAccountLockoutTimeSpan = TimeSpan.FromSeconds(GetSettingValue<int>(StudioXZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds, tenantId));
            MaxFailedAccessAttemptsBeforeLockout = GetSettingValue<int>(StudioXZeroSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout, tenantId);
        }

        public override async Task<IList<string>> GetValidTwoFactorProvidersAsync(long userId)
        {
            var user = await GetUserByIdAsync(userId);

            RegisterTwoFactorProviders(user.TenantId);

            return await base.GetValidTwoFactorProvidersAsync(userId);
        }

        public override async Task<IdentityResult> NotifyTwoFactorTokenAsync(long userId, string twoFactorProvider, string token)
        {
            var user = await GetUserByIdAsync(userId);

            RegisterTwoFactorProviders(user.TenantId);

            return await base.NotifyTwoFactorTokenAsync(userId, twoFactorProvider, token);
        }

        public override async Task<string> GenerateTwoFactorTokenAsync(long userId, string twoFactorProvider)
        {
            var user = await GetUserByIdAsync(userId);

            RegisterTwoFactorProviders(user.TenantId);

            return await base.GenerateTwoFactorTokenAsync(userId, twoFactorProvider);
        }

        public override async Task<bool> VerifyTwoFactorTokenAsync(long userId, string twoFactorProvider, string token)
        {
            var user = await GetUserByIdAsync(userId);

            RegisterTwoFactorProviders(user.TenantId);

            return await base.VerifyTwoFactorTokenAsync(userId, twoFactorProvider, token);
        }

        protected virtual Task<string> GetOldUserNameAsync(long userId)
        {
            return StudioXStore.GetUserNameFromDatabaseAsync(userId);
        }

        private async Task<UserPermissionCacheItem> GetUserPermissionCacheItemAsync(long userId)
        {
            var cacheKey = userId + "@" + (GetCurrentTenantId() ?? 0);
            return await cacheManager.GetUserPermissionCache().GetAsync(cacheKey, async () =>
            {
                var user = await FindByIdAsync(userId);
                if (user == null)
                {
                    return null;
                }

                var newCacheItem = new UserPermissionCacheItem(userId);

                foreach (var roleName in await GetRolesAsync(userId))
                {
                    newCacheItem.RoleIds.Add((await RoleManager.GetRoleByNameAsync(roleName)).Id);
                }

                foreach (var permissionInfo in await UserPermissionStore.GetPermissionsAsync(userId))
                {
                    if (permissionInfo.IsGranted)
                    {
                        newCacheItem.GrantedPermissions.Add(permissionInfo.Name);
                    }
                    else
                    {
                        newCacheItem.ProhibitedPermissions.Add(permissionInfo.Name);
                    }
                }

                return newCacheItem;
            });
        }

        private bool IsTrue(string settingName, int? tenantId)
        {
            return GetSettingValue<bool>(settingName, tenantId);
        }

        private T GetSettingValue<T>(string settingName, int? tenantId) where T : struct
        {
            return tenantId == null
                ? settingManager.GetSettingValueForApplication<T>(settingName)
                : settingManager.GetSettingValueForTenant<T>(settingName, tenantId.Value);
        }

        private string L(string name)
        {
            return LocalizationManager.GetString(StudioXZeroConsts.LocalizationSourceName, name);
        }

        private int? GetCurrentTenantId()
        {
            if (unitOfWorkManager.Current != null)
            {
                return unitOfWorkManager.Current.GetTenantId();
            }

            return StudioXSession.TenantId;
        }
    }
}