using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using StudioX.Auditing;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.Configuration;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.IdentityFramework;
using StudioX.MultiTenancy;
using StudioX.Timing;
using StudioX.Zero.Configuration;
using Microsoft.AspNetCore.Identity;

namespace StudioX.Authorization
{
    public class StudioXLogInManager<TTenant, TRole, TUser> : ITransientDependency
        where TTenant : StudioXTenant<TUser>
        where TRole : StudioXRole<TUser>, new()
        where TUser : StudioXUser<TUser>
    {
        public IClientInfoProvider ClientInfoProvider { get; set; }

        protected IMultiTenancyConfig MultiTenancyConfig { get; }
        protected IRepository<TTenant> TenantRepository { get; }
        protected IUnitOfWorkManager UnitOfWorkManager { get; }
        protected StudioXUserManager<TRole, TUser> UserManager { get; }
        protected ISettingManager SettingManager { get; }
        protected IRepository<UserLoginAttempt, long> UserLoginAttemptRepository { get; }
        protected IUserManagementConfig UserManagementConfig { get; }
        protected IIocResolver IocResolver { get; }
        protected StudioXRoleManager<TRole, TUser> RoleManager { get; }

        private readonly IPasswordHasher<TUser> passwordHasher;

        private readonly StudioXUserClaimsPrincipalFactory<TUser, TRole> claimsPrincipalFactory;

        public StudioXLogInManager(
            StudioXUserManager<TRole, TUser> userManager,
            IMultiTenancyConfig multiTenancyConfig,
            IRepository<TTenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver, 
            IPasswordHasher<TUser> passwordHasher,
            StudioXRoleManager<TRole, TUser> roleManager,
            StudioXUserClaimsPrincipalFactory<TUser, TRole> claimsPrincipalFactory)
        {
            this.passwordHasher = passwordHasher;
            this.claimsPrincipalFactory = claimsPrincipalFactory;
            MultiTenancyConfig = multiTenancyConfig;
            TenantRepository = tenantRepository;
            UnitOfWorkManager = unitOfWorkManager;
            SettingManager = settingManager;
            UserLoginAttemptRepository = userLoginAttemptRepository;
            UserManagementConfig = userManagementConfig;
            IocResolver = iocResolver;
            RoleManager = roleManager;
            UserManager = userManager;

            ClientInfoProvider = NullClientInfoProvider.Instance;
        }

        [UnitOfWork]
        public virtual async Task<StudioXLoginResult<TTenant, TUser>> LoginAsync(UserLoginInfo login, string tenancyName = null)
        {
            var result = await LoginAsyncInternal(login, tenancyName);
            await SaveLoginAttempt(result, tenancyName, login.ProviderKey + "@" + login.LoginProvider);
            return result;
        }

        protected virtual async Task<StudioXLoginResult<TTenant, TUser>> LoginAsyncInternal(UserLoginInfo login, string tenancyName)
        {
            if (login == null || login.LoginProvider.IsNullOrEmpty() || login.ProviderKey.IsNullOrEmpty())
            {
                throw new ArgumentException("login");
            }

            //Get and check tenant
            TTenant tenant = null;
            if (!MultiTenancyConfig.IsEnabled)
            {
                tenant = await GetDefaultTenantAsync();
            }
            else if (!string.IsNullOrWhiteSpace(tenancyName))
            {
                tenant = await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenancyName);
                if (tenant == null)
                {
                    return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.InvalidTenancyName);
                }

                if (!tenant.IsActive)
                {
                    return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.TenantIsNotActive, tenant);
                }
            }

            int? tenantId = tenant == null ? (int?)null : tenant.Id;
            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var user = await UserManager.StudioXStore.FindAsync(tenantId, login);
                if (user == null)
                {
                    return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.UnknownExternalLogin, tenant);
                }

                return await CreateLoginResultAsync(user, tenant);
            }
        }

        [UnitOfWork]
        public virtual async Task<StudioXLoginResult<TTenant, TUser>> LoginAsync(string userNameOrEmailAddress, string plainPassword, string tenancyName = null, bool shouldLockout = true)
        {
            var result = await LoginAsyncInternal(userNameOrEmailAddress, plainPassword, tenancyName, shouldLockout);
            await SaveLoginAttempt(result, tenancyName, userNameOrEmailAddress);
            return result;
        }

        protected virtual async Task<StudioXLoginResult<TTenant, TUser>> LoginAsyncInternal(string userNameOrEmailAddress, string plainPassword, string tenancyName, bool shouldLockout)
        {
            if (userNameOrEmailAddress.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(userNameOrEmailAddress));
            }

            if (plainPassword.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(plainPassword));
            }

            //Get and check tenant
            TTenant tenant = null;
            using (UnitOfWorkManager.Current.SetTenantId(null))
            {
                if (!MultiTenancyConfig.IsEnabled)
                {
                    tenant = await GetDefaultTenantAsync();
                }
                else if (!string.IsNullOrWhiteSpace(tenancyName))
                {
                    tenant = await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenancyName);
                    if (tenant == null)
                    {
                        return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.InvalidTenancyName);
                    }

                    if (!tenant.IsActive)
                    {
                        return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.TenantIsNotActive, tenant);
                    }
                }
            }

            var tenantId = tenant == null ? (int?)null : tenant.Id;
            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                await UserManager.InitializeOptionsAsync(tenantId);

                //TryLoginFromExternalAuthenticationSources method may create the user, that's why we are calling it before StudioXStore.FindByNameOrEmailAsync
                var loggedInFromExternalSource = await TryLoginFromExternalAuthenticationSources(userNameOrEmailAddress, plainPassword, tenant);

                var user = await UserManager.StudioXStore.FindByNameOrEmailAsync(tenantId, userNameOrEmailAddress);
                if (user == null)
                {
                    return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.InvalidUserNameOrEmailAddress, tenant);
                }

                if (!loggedInFromExternalSource)
                {
                    if (await UserManager.IsLockedOutAsync(user))
                    {
                        return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.LockedOut, tenant, user);
                    }

                    if (!await UserManager.CheckPasswordAsync(user, plainPassword))
                    {
                        if (shouldLockout)
                        {
                            if (await TryLockOutAsync(tenantId, user.Id))
                            {
                                return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.LockedOut, tenant, user);
                            }
                        }

                        return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.InvalidPassword, tenant, user);
                    }
                    
                    await UserManager.ResetAccessFailedCountAsync(user);
                }

                return await CreateLoginResultAsync(user, tenant);
            }
        }

        protected virtual async Task<StudioXLoginResult<TTenant, TUser>> CreateLoginResultAsync(TUser user, TTenant tenant = null)
        {
            if (!user.IsActive)
            {
                return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.UserIsNotActive);
            }

            if (await IsEmailConfirmationRequiredForLoginAsync(user.TenantId) && !user.IsEmailConfirmed)
            {
                return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.UserEmailIsNotConfirmed);
            }

            if (await IsPhoneConfirmationRequiredForLoginAsync(user.TenantId) && !user.IsPhoneNumberConfirmed)
            {
                return new StudioXLoginResult<TTenant, TUser>(StudioXLoginResultType.UserPhoneNumberIsNotConfirmed);
            }

            user.LastLoginTime = Clock.Now;

            await UserManager.StudioXStore.UpdateAsync(user);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            var principal = await claimsPrincipalFactory.CreateAsync(user);

            return new StudioXLoginResult<TTenant, TUser>(
                tenant,
                user,
                principal.Identity as ClaimsIdentity
            );
        }

        protected virtual async Task SaveLoginAttempt(StudioXLoginResult<TTenant, TUser> loginResult, string tenancyName, string userNameOrEmailAddress)
        {
            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                var tenantId = loginResult.Tenant != null ? loginResult.Tenant.Id : (int?)null;
                using (UnitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var loginAttempt = new UserLoginAttempt
                    {
                        TenantId = tenantId,
                        TenancyName = tenancyName,

                        UserId = loginResult.User != null ? loginResult.User.Id : (long?) null,
                        UserNameOrEmailAddress = userNameOrEmailAddress,

                        Result = loginResult.Result,

                        BrowserInfo = ClientInfoProvider.BrowserInfo,
                        ClientIpAddress = ClientInfoProvider.ClientIpAddress,
                        ClientName = ClientInfoProvider.ComputerName,
                    };

                    await UserLoginAttemptRepository.InsertAsync(loginAttempt);
                    await UnitOfWorkManager.Current.SaveChangesAsync();

                    await uow.CompleteAsync();
                }
            }
        }

        protected virtual async Task<bool> TryLockOutAsync(int? tenantId, long userId)
        {
            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                using (UnitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var user = await UserManager.FindByIdAsync(userId.ToString());

                    (await UserManager.AccessFailedAsync(user)).CheckErrors();

                    var isLockOut =  await UserManager.IsLockedOutAsync(user);

                    await UnitOfWorkManager.Current.SaveChangesAsync();

                    await uow.CompleteAsync();

                    return isLockOut;
                }
            }
        }

        protected virtual async Task<bool> TryLoginFromExternalAuthenticationSources(string userNameOrEmailAddress, string plainPassword, TTenant tenant)
        {
            if (!UserManagementConfig.ExternalAuthenticationSources.Any())
            {
                return false;
            }

            foreach (var sourceType in UserManagementConfig.ExternalAuthenticationSources)
            {
                using (var source = IocResolver.ResolveAsDisposable<IExternalAuthenticationSource<TTenant, TUser>>(sourceType))
                {
                    if (await source.Object.TryAuthenticateAsync(userNameOrEmailAddress, plainPassword, tenant))
                    {
                        var tenantId = tenant == null ? (int?)null : tenant.Id;
                        using (UnitOfWorkManager.Current.SetTenantId(tenantId))
                        {
                            var user = await UserManager.StudioXStore.FindByNameOrEmailAsync(tenantId, userNameOrEmailAddress);
                            if (user == null)
                            {
                                user = await source.Object.CreateUserAsync(userNameOrEmailAddress, tenant);

                                user.TenantId = tenantId;
                                user.AuthenticationSource = source.Object.Name;
                                user.Password = passwordHasher.HashPassword(user, Guid.NewGuid().ToString("N").Left(16)); //Setting a random password since it will not be used

                                if (user.Roles == null)
                                {
                                    user.Roles = new List<UserRole>();
                                    foreach (var defaultRole in RoleManager.Roles.Where(r => r.TenantId == tenantId && r.IsDefault).ToList())
                                    {
                                        user.Roles.Add(new UserRole(tenantId, user.Id, defaultRole.Id));
                                    }
                                }
                                
                                await UserManager.StudioXStore.CreateAsync(user);
                            }
                            else
                            {
                                await source.Object.UpdateUserAsync(user, tenant);

                                user.AuthenticationSource = source.Object.Name;

                                await UserManager.StudioXStore.UpdateAsync(user);
                            }

                            await UnitOfWorkManager.Current.SaveChangesAsync();

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected virtual async Task<TTenant> GetDefaultTenantAsync()
        {
            var tenant = await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == StudioXTenant<TUser>.DefaultTenantName);
            if (tenant == null)
            {
                throw new StudioXException("There should be a 'Default' tenant if multi-tenancy is disabled!");
            }

            return tenant;
        }

        protected virtual async Task<bool> IsEmailConfirmationRequiredForLoginAsync(int? tenantId)
        {
            if (tenantId.HasValue)
            {
                return await SettingManager.GetSettingValueForTenantAsync<bool>(StudioXZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin, tenantId.Value);
            }

            return await SettingManager.GetSettingValueForApplicationAsync<bool>(StudioXZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);
        }

        protected virtual Task<bool> IsPhoneConfirmationRequiredForLoginAsync(int? tenantId)
        {
            return Task.FromResult(false);
        }
    }
}
