﻿using System;
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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace StudioX.Authorization
{
    public class StudioXSignInManager<TTenant, TRole, TUser> : SignInManager<TUser>, ITransientDependency
        where TTenant : StudioXTenant<TUser>
        where TRole : StudioXRole<TUser>, new()
        where TUser : StudioXUser<TUser>
    {
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly ISettingManager settingManager;

        public StudioXSignInManager(
            StudioXUserManager<TRole, TUser> userManager,
            IHttpContextAccessor contextAccessor,
            StudioXUserClaimsPrincipalFactory<TUser, TRole> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager)
            : base(
                userManager,
                contextAccessor,
                claimsFactory,
                optionsAccessor,
                logger)
        {
            this.unitOfWorkManager = unitOfWorkManager;
            this.settingManager = settingManager;
        }

        public virtual async Task<SignInResult> SignInOrTwoFactorAsync(StudioXLoginResult<TTenant, TUser> loginResult, bool isPersistent, bool? rememberBrowser = null, string loginProvider = null, bool bypassTwoFactor = false)
        {
            if (loginResult.Result != StudioXLoginResultType.Success)
            {
                throw new ArgumentException("loginResult.Result should be success in order to sign in!");
            }

            using (unitOfWorkManager.Current.SetTenantId(loginResult.Tenant?.Id))
            {
                await UserManager.As<StudioXUserManager<TRole, TUser>>().InitializeOptionsAsync(loginResult.Tenant?.Id);

                if (!bypassTwoFactor && IsTrue(StudioXZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled, loginResult.Tenant?.Id))
                {
                    if (await UserManager.GetTwoFactorEnabledAsync(loginResult.User))
                    {
                        if ((await UserManager.GetValidTwoFactorProvidersAsync(loginResult.User)).Count > 0)
                        {
                            if (!await IsTwoFactorClientRememberedAsync(loginResult.User) || rememberBrowser == false)
                            {
                                await Context.Authentication.SignInAsync(
                                    Options.Cookies.TwoFactorUserIdCookieAuthenticationScheme,
                                    StoreTwoFactorInfo(loginResult.User, loginProvider)
                                );

                                return SignInResult.TwoFactorRequired;
                            }
                        }
                    }
                }

                if (loginProvider != null)
                {
                    await Context.Authentication.SignOutAsync(Options.Cookies.ExternalCookieAuthenticationScheme);
                }

                await SignInAsync(loginResult.User, isPersistent, loginProvider);
                return SignInResult.Success;
            }
        }

        public virtual async Task SignOutAndSignInAsync(ClaimsIdentity identity, bool isPersistent)
        {
            await SignOutAsync();
            await SignInAsync(identity, isPersistent);
        }

        public virtual async Task SignInAsync(ClaimsIdentity identity, bool isPersistent)
        {
            await Context.Authentication.SignInAsync(Options.Cookies.ApplicationCookieAuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties {IsPersistent = isPersistent}
            );
        }

        [UnitOfWork]
        public override Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            return base.SignInAsync(user, authenticationProperties, authenticationMethod);
        }

        protected virtual ClaimsPrincipal StoreTwoFactorInfo(TUser user, string loginProvider)
        {
            var identity = new ClaimsIdentity(Options.Cookies.TwoFactorUserIdCookieAuthenticationScheme);

            identity.AddClaim(new Claim(ClaimTypes.Name, user.Id.ToString()));

            if (user.TenantId.HasValue)
            {
                identity.AddClaim(new Claim(StudioXClaimTypes.TenantId, user.TenantId.Value.ToString()));
            }

            if (loginProvider != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, loginProvider));
            }

            return new ClaimsPrincipal(identity);
        }

        public async Task<int?> GetVerifiedTenantIdAsync()
        {
            var principal = await Context.Authentication.AuthenticateAsync(Options.Cookies.TwoFactorUserIdCookieAuthenticationScheme);

            if (principal == null)
            {
                return null;
            }

            return StudioXZeroClaimsIdentityHelper.GetTenantId(principal);
        }

        public override async Task<bool> IsTwoFactorClientRememberedAsync(TUser user)
        {
            var result = await Context.Authentication.AuthenticateAsync(Options.Cookies.TwoFactorRememberMeCookieAuthenticationScheme);

            return result != null &&
                   result.FindFirstValue(ClaimTypes.Name) == user.Id.ToString() &&
                   StudioXZeroClaimsIdentityHelper.GetTenantId(result) == user.TenantId;
        }

        public override async Task RememberTwoFactorClientAsync(TUser user)
        {
            var rememberBrowserIdentity = new ClaimsIdentity(Options.Cookies.TwoFactorRememberMeCookieAuthenticationScheme);

            rememberBrowserIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Id.ToString()));

            if (user.TenantId.HasValue)
            {
                rememberBrowserIdentity.AddClaim(new Claim(StudioXClaimTypes.TenantId, user.TenantId.Value.ToString()));
            }

            await Context.Authentication.SignInAsync(Options.Cookies.TwoFactorRememberMeCookieAuthenticationScheme,
                new ClaimsPrincipal(rememberBrowserIdentity),
                new AuthenticationProperties { IsPersistent = true });
        }

        private bool IsTrue(string settingName, int? tenantId)
        {
            return tenantId == null
                ? settingManager.GetSettingValueForApplication<bool>(settingName)
                : settingManager.GetSettingValueForTenant<bool>(settingName, tenantId.Value);
        }
    }
}
