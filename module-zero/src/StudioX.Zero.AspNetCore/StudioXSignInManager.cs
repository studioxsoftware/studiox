using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.Configuration;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.MultiTenancy;
using StudioX.Runtime.Security;
using StudioX.Zero.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;

namespace StudioX.Zero.AspNetCore
{
    public abstract class StudioXSignInManager<TTenant, TRole, TUser> : ITransientDependency
           where TTenant : StudioXTenant<TUser>
           where TRole : StudioXRole<TUser>, new()
           where TUser : StudioXUser<TUser>
    {
        public StudioXUserManager<TRole, TUser> UserManager { get; set; }

        public AuthenticationManager AuthenticationManager => contextAccessor.HttpContext.Authentication;

        private readonly IHttpContextAccessor contextAccessor;
        private readonly ISettingManager settingManager;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly IStudioXZeroAspNetCoreConfiguration configuration;

        protected StudioXSignInManager(
            StudioXUserManager<TRole, TUser> userManager,
            IHttpContextAccessor contextAccessor,
            ISettingManager settingManager,
            IUnitOfWorkManager unitOfWorkManager,
            IStudioXZeroAspNetCoreConfiguration configuration)
        {
            UserManager = userManager;
            this.contextAccessor = contextAccessor;
            this.settingManager = settingManager;
            this.unitOfWorkManager = unitOfWorkManager;
            this.configuration = configuration;
        }

        /// <summary>
        /// This method can return two results:
        /// <see cref="SignInStatus.Success"/> indicates that user has successfully signed in.
        /// <see cref="SignInStatus.RequiresVerification"/> indicates that user has successfully signed in.
        /// </summary>
        /// <param name="loginResult">The login result received from <see cref="StudioXLogInManager{TTenant,TRole,TUser}"/> Should be Success.</param>
        /// <param name="isPersistent">True to use persistent cookie.</param>
        /// <param name="rememberBrowser">Remember user's browser (and not use two factor auth again) or not.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">loginResult.Result should be success in order to sign in!</exception>
        [UnitOfWork]
        public virtual async Task<SignInStatus> SignInOrTwoFactorAsync(StudioXLoginResult<TTenant, TUser> loginResult, bool isPersistent, bool? rememberBrowser = null)
        {
            if (loginResult.Result != StudioXLoginResultType.Success)
            {
                throw new ArgumentException("loginResult.Result should be success in order to sign in!");
            }

            using (unitOfWorkManager.Current.SetTenantId(loginResult.Tenant?.Id))
            {
                if (IsTrue(StudioXZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled, loginResult.Tenant?.Id))
                {
                    UserManager.As<StudioXUserManager<TRole, TUser>>().RegisterTwoFactorProviders(loginResult.Tenant?.Id);

                    if (await UserManager.GetTwoFactorEnabledAsync(loginResult.User.Id))
                    {
                        if ((await UserManager.GetValidTwoFactorProvidersAsync(loginResult.User.Id)).Count > 0)
                        {
                            if (!await TwoFactorBrowserRememberedAsync(loginResult.User.Id.ToString(), loginResult.User.TenantId) ||
                                rememberBrowser == false)
                            {
                                var claimsIdentity = new ClaimsIdentity(configuration.TwoFactorAuthenticationScheme);

                                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginResult.User.Id.ToString()));

                                if (loginResult.Tenant != null)
                                {
                                    claimsIdentity.AddClaim(new Claim(StudioXClaimTypes.TenantId, loginResult.Tenant.Id.ToString()));
                                }

                                await AuthenticationManager.SignInAsync(
                                    configuration.TwoFactorAuthenticationScheme,
                                    CreateIdentityForTwoFactor(
                                        loginResult.User,
                                        configuration.TwoFactorAuthenticationScheme
                                    )
                                );

                                return SignInStatus.RequiresVerification;
                            }
                        }
                    }
                }

                await SignInAsync(loginResult, isPersistent, rememberBrowser);
                return SignInStatus.Success;
            }
        }

        /// <param name="loginResult">The login result received from <see cref="StudioXLogInManager{TTenant,TRole,TUser}"/> Should be Success.</param>
        /// <param name="isPersistent">True to use persistent cookie.</param>
        /// <param name="rememberBrowser">Remember user's browser (and not use two factor auth again) or not.</param>
        [UnitOfWork]
        public virtual async Task SignInAsync(StudioXLoginResult<TTenant, TUser> loginResult, bool isPersistent, bool? rememberBrowser = null)
        {
            if (loginResult.Result != StudioXLoginResultType.Success)
            {
                throw new ArgumentException("loginResult.Result should be success in order to sign in!");
            }

            using (unitOfWorkManager.Current.SetTenantId(loginResult.Tenant?.Id))
            {
                await SignOutAllAsync();

                if (rememberBrowser == null)
                {
                    rememberBrowser = IsTrue(StudioXZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled, loginResult.Tenant?.Id);
                }

                if (rememberBrowser == true)
                {
                    await SignOutAllAndSignInAsync(loginResult.Identity, isPersistent);
                    await AuthenticationManager.SignInAsync(
                        configuration.TwoFactorRememberBrowserAuthenticationScheme,
                        CreateIdentityForTwoFactor(
                            loginResult.User,
                            configuration.TwoFactorRememberBrowserAuthenticationScheme
                        )
                    );
                }
                else
                {
                    await SignOutAllAndSignInAsync(loginResult.Identity, isPersistent);
                }
            }
        }

        private static ClaimsPrincipal CreateIdentityForTwoFactor(TUser user, string authType)
        {
            var claimsIdentity = new ClaimsIdentity(authType);

            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

            if (user.TenantId.HasValue)
            {
                claimsIdentity.AddClaim(new Claim(StudioXClaimTypes.TenantId, user.TenantId.Value.ToString()));
            }

            return new ClaimsPrincipal(claimsIdentity);
        }

        public virtual async Task SignInAsync(TUser user, bool isPersistent, bool? rememberBrowser = null)
        {
            using (unitOfWorkManager.Current.SetTenantId(user.TenantId))
            {
                var userIdentity = await UserManager.CreateIdentityAsync(user, configuration.AuthenticationScheme);

                await SignOutAllAsync();

                if (rememberBrowser == null)
                {
                    rememberBrowser = IsTrue(StudioXZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled, user.TenantId);
                }

                if (rememberBrowser == true)
                {
                    await SignOutAllAndSignInAsync(userIdentity, isPersistent);
                    await AuthenticationManager.SignInAsync(
                        configuration.TwoFactorRememberBrowserAuthenticationScheme,
                        CreateIdentityForTwoFactor(
                            user,
                            configuration.TwoFactorRememberBrowserAuthenticationScheme
                        )
                    );
                }
                else
                {
                    await SignOutAllAndSignInAsync(userIdentity, isPersistent);
                }
            }
        }

        public virtual async Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser)
        {
            var userId = await GetVerifiedUserIdAsync();
            if (userId <= 0)
            {
                return SignInStatus.Failure;
            }

            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return SignInStatus.Failure;
            }

            if (await UserManager.IsLockedOutAsync(user.Id))
            {
                return SignInStatus.LockedOut;
            }

            if (await UserManager.VerifyTwoFactorTokenAsync(user.Id, provider, code))
            {
                await UserManager.ResetAccessFailedCountAsync(user.Id);
                await SignInAsync(user, isPersistent, rememberBrowser);
                return SignInStatus.Success;
            }

            await UserManager.AccessFailedAsync(user.Id);
            return SignInStatus.Failure;
        }

        public virtual async Task<bool> SendTwoFactorCodeAsync(string provider)
        {
            var userId = await GetVerifiedUserIdAsync();
            if (userId <= 0)
            {
                return false;
            }

            var token = await UserManager.GenerateTwoFactorTokenAsync(userId, provider);
            var identityResult = await UserManager.NotifyTwoFactorTokenAsync(userId, provider, token);
            return identityResult == IdentityResult.Success;
        }

        public async Task<long> GetVerifiedUserIdAsync()
        {
            var authenticateResult = await AuthenticationManager.AuthenticateAsync(configuration.TwoFactorAuthenticationScheme);
            return Convert.ToInt64(IdentityExtensions.GetUserId(authenticateResult.Identity) ?? "0");
        }

        public virtual async Task<int?> GetVerifiedTenantIdAsync()
        {
            var authenticateResult = await AuthenticationManager.AuthenticateAsync(configuration.TwoFactorAuthenticationScheme);
            return StudioXZeroClaimsIdentityHelper.GetTenantId(authenticateResult?.Identity);
        }

        public async Task<bool> TwoFactorBrowserRememberedAsync(string userId, int? tenantId)
        {
            var result = await AuthenticationManager.AuthenticateAsync(configuration.TwoFactorRememberBrowserAuthenticationScheme);
            if (result?.Identity == null)
            {
                return false;
            }

            if (IdentityExtensions.GetUserId(result.Identity) != userId)
            {
                return false;
            }

            if (StudioXZeroClaimsIdentityHelper.GetTenantId(result.Identity) != tenantId)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> HasBeenVerifiedAsync()
        {
            return await GetVerifiedUserIdAsync() > 0;
        }

        private bool IsTrue(string settingName, int? tenantId)
        {
            return tenantId == null
                ? settingManager.GetSettingValueForApplication<bool>(settingName)
                : settingManager.GetSettingValueForTenant<bool>(settingName, tenantId.Value);
        }

        public async Task SignOutAllAsync()
        {
            await AuthenticationManager.SignOutAsync(configuration.AuthenticationScheme);
            await AuthenticationManager.SignOutAsync(configuration.TwoFactorAuthenticationScheme);
        }

        public async Task SignOutAllAndSignInAsync(ClaimsIdentity identity, bool rememberMe = false)
        {
            await SignOutAllAsync();
            await AuthenticationManager.SignInAsync(
                configuration.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe
                }
            );
        }

        public async Task<ExternalLoginUserInfo> GetExternalLoginUserInfo(string authSchema)
        {
            var authInfo = await AuthenticationManager.GetAuthenticateInfoAsync(authSchema);
            if (authInfo == null)
            {
                return null;
            }

            var claims = authInfo.Principal.Claims.ToList();

            var userInfo = new ExternalLoginUserInfo
            {
                LoginInfo = new UserLoginInfo(
                    authInfo.Properties.Items["LoginProvider"],
                    authInfo.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                )
            };

            var givennameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
            if (givennameClaim != null && !givennameClaim.Value.IsNullOrEmpty())
            {
                userInfo.FirstName = givennameClaim.Value;
            }

            var lastNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
            if (lastNameClaim != null && !lastNameClaim.Value.IsNullOrEmpty())
            {
                userInfo.LastName = lastNameClaim.Value;
            }

            if (userInfo.FirstName == null || userInfo.LastName == null)
            {
                var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (nameClaim != null)
                {
                    var fullName = nameClaim.Value;
                    if (!fullName.IsNullOrEmpty())
                    {
                        var lastSpaceIndex = fullName.LastIndexOf(' ');
                        if (lastSpaceIndex < 1 || lastSpaceIndex > (fullName.Length - 2))
                        {
                            userInfo.FirstName = userInfo.LastName = fullName;
                        }
                        else
                        {
                            userInfo.FirstName = fullName.Substring(0, lastSpaceIndex);
                            userInfo.LastName = fullName.Substring(lastSpaceIndex);
                        }
                    }
                }
            }

            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim != null)
            {
                userInfo.EmailAddress = emailClaim.Value;
            }

            return userInfo;
        }
    }
}
