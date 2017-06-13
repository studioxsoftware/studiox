using System;
using System.Security.Claims;
using System.Threading.Tasks;
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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace StudioX.Authorization
{
    public abstract class StudioXSignInManager<TTenant, TRole, TUser> : SignInManager<TUser, long>, ITransientDependency
        where TTenant : StudioXTenant<TUser>
        where TRole : StudioXRole<TUser>, new()
        where TUser : StudioXUser<TUser>
    {
        private readonly ISettingManager settingManager;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        protected StudioXSignInManager(
            StudioXUserManager<TRole, TUser> userManager,
            IAuthenticationManager authenticationManager,
            ISettingManager settingManager,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
                  userManager,
                  authenticationManager)
        {
            this.settingManager = settingManager;
            this.unitOfWorkManager = unitOfWorkManager;
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
        public virtual async Task<SignInStatus> SignInOrTwoFactor(StudioXLoginResult<TTenant, TUser> loginResult, bool isPersistent, bool? rememberBrowser = null)
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
                            if (!await AuthenticationManager.TwoFactorBrowserRememberedAsync(loginResult.User.Id.ToString()) || 
                                rememberBrowser == false)
                            {
                                var claimsIdentity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);

                                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, loginResult.User.Id.ToString()));

                                if (loginResult.Tenant != null)
                                {
                                    claimsIdentity.AddClaim(new Claim(StudioXClaimTypes.TenantId, loginResult.Tenant.Id.ToString()));
                                }

                                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claimsIdentity);
                                return SignInStatus.RequiresVerification;
                            }
                        }
                    }
                }

                SignIn(loginResult, isPersistent, rememberBrowser);
                return SignInStatus.Success;
            }
        }

        /// <param name="loginResult">The login result received from <see cref="StudioXLogInManager{TTenant,TRole,TUser}"/> Should be Success.</param>
        /// <param name="isPersistent">True to use persistent cookie.</param>
        /// <param name="rememberBrowser">Remember user's browser (and not use two factor auth again) or not.</param>
        [UnitOfWork]
        public virtual void SignIn(StudioXLoginResult<TTenant, TUser> loginResult, bool isPersistent, bool? rememberBrowser = null)
        {
            if (loginResult.Result != StudioXLoginResultType.Success)
            {
                throw new ArgumentException("loginResult.Result should be success in order to sign in!");
            }

            using (unitOfWorkManager.Current.SetTenantId(loginResult.Tenant?.Id))
            {
                AuthenticationManager.SignOut(
                    DefaultAuthenticationTypes.ExternalCookie,
                    DefaultAuthenticationTypes.TwoFactorCookie
                );

                if (rememberBrowser == null)
                {
                    rememberBrowser = IsTrue(StudioXZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled, loginResult.Tenant?.Id);
                }

                if (rememberBrowser == true)
                {
                    var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(loginResult.User.Id.ToString());
                    AuthenticationManager.SignIn(
                        new AuthenticationProperties
                        {
                            IsPersistent = isPersistent
                        },
                        loginResult.Identity,
                        rememberBrowserIdentity
                    );
                }
                else
                {
                    AuthenticationManager.SignIn(
                        new AuthenticationProperties
                        {
                            IsPersistent = isPersistent
                        },
                        loginResult.Identity
                    );
                }
            }
        }

        public virtual async Task<int?> GetVerifiedTenantIdAsync()
        {
            var authenticateResult = await AuthenticationManager.AuthenticateAsync(
                DefaultAuthenticationTypes.TwoFactorCookie
            );

            return authenticateResult?.Identity?.GetTenantId();
        }

        private bool IsTrue(string settingName, int? tenantId)
        {
            return tenantId == null
                ? settingManager.GetSettingValueForApplication<bool>(settingName)
                : settingManager.GetSettingValueForTenant<bool>(settingName, tenantId.Value);
        }
    }
}
