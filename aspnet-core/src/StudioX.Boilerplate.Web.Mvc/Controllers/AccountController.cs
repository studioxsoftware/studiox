using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using StudioX.Authorization;
using StudioX.Authorization.Users;
using StudioX.Configuration;
using StudioX.Configuration.Startup;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.MultiTenancy;
using StudioX.Notifications;
using StudioX.Threading;
using StudioX.Timing;
using StudioX.UI;
using StudioX.Web.Models;
using StudioX.Zero.Configuration;
using Microsoft.AspNetCore.Mvc;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.MultiTenancy;
using StudioX.Boilerplate.Web.Models.Account;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Controllers;
using StudioX.Boilerplate.Identity;
using StudioX.Boilerplate.Sessions;
using StudioX.Boilerplate.Web.Views.Shared.Components.TenantChange;
using Microsoft.AspNetCore.Identity;

namespace StudioX.Boilerplate.Web.Controllers
{
    public class AccountController : BoilerplateControllerBase
    {
        private readonly UserManager userManager;
        private readonly TenantManager tenantManager;
        private readonly IMultiTenancyConfig multiTenancyConfig;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly StudioXLoginResultTypeHelper studioXLoginResultTypeHelper;
        private readonly LogInManager logInManager;
        private readonly SignInManager signInManager;
        private readonly UserRegistrationManager userRegistrationManager;
        private readonly ISessionAppService sessionAppService;
        private readonly ITenantCache tenantCache;
        private readonly INotificationPublisher notificationPublisher;

        public AccountController(
            UserManager userManager,
            IMultiTenancyConfig multiTenancyConfig,
            TenantManager tenantManager,
            IUnitOfWorkManager unitOfWorkManager,
            StudioXLoginResultTypeHelper loginResultTypeHelper,
            LogInManager logInManager,
            SignInManager signInManager,
            UserRegistrationManager userRegistrationManager,
            ISessionAppService sessionAppService,
            ITenantCache tenantCache,
            INotificationPublisher notificationPublisher)
        {
            this.userManager = userManager;
            this.multiTenancyConfig = multiTenancyConfig;
            this.tenantManager = tenantManager;
            this.unitOfWorkManager = unitOfWorkManager;
            this.studioXLoginResultTypeHelper = loginResultTypeHelper;
            this.logInManager = logInManager;
            this.signInManager = signInManager;
            this.userRegistrationManager = userRegistrationManager;
            this.sessionAppService = sessionAppService;
            this.tenantCache = tenantCache;
            this.notificationPublisher = notificationPublisher;
        }

        #region Login / Logout

        public ActionResult Login(string userNameOrEmailAddress = "", string returnUrl = "", string successMessage = "")
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = GetAppHomeUrl();
            }

            return View(new LoginFormViewModel
            {
                ReturnUrl = returnUrl,
                IsMultiTenancyEnabled = multiTenancyConfig.IsEnabled,
                IsSelfRegistrationAllowed = IsSelfRegistrationEnabled(),
                MultiTenancySide = StudioXSession.MultiTenancySide
            });
        }

        [HttpPost]
        [UnitOfWork]
        public virtual async Task<JsonResult> Login(LoginViewModel loginModel, string returnUrl = "", string returnUrlHash = "")
        {
            returnUrl = NormalizeReturnUrl(returnUrl);
            if (!string.IsNullOrWhiteSpace(returnUrlHash))
            {
                returnUrl = returnUrl + returnUrlHash;
            }

            var loginResult = await GetLoginResultAsync(loginModel.UsernameOrEmailAddress, loginModel.Password, GetTenancyNameOrNull());

            await signInManager.SignInAsync(loginResult.Identity, loginModel.RememberMe);
            await UnitOfWorkManager.Current.SaveChangesAsync();

            return Json(new AjaxResponse { TargetUrl = returnUrl });
        }

        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }


        private async Task<StudioXLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case StudioXLoginResultType.Success:
                    return loginResult;
                default:
                    throw studioXLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        #endregion

        #region Register

        public ActionResult Register()
        {
            return RegisterView(new RegisterViewModel());
        }

        private ActionResult RegisterView(RegisterViewModel model)
        {
            ViewBag.IsMultiTenancyEnabled = multiTenancyConfig.IsEnabled;

            return View("Register", model);
        }

        private bool IsSelfRegistrationEnabled()
        {
            if (!StudioXSession.TenantId.HasValue)
            {
                return false; //No registration enabled for host users!
            }

            return true;
        }

        [HttpPost]
        [UnitOfWork]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                ExternalLoginInfo externalLoginInfo = null;
                if (model.IsExternalLogin)
                {
                    externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();
                    if (externalLoginInfo == null)
                    {
                        throw new Exception("Can not external login!");
                    }

                    model.UserName = model.EmailAddress;
                    model.Password = Authorization.Users.User.CreateRandomPassword();
                }
                else
                {
                    if (model.UserName.IsNullOrEmpty() || model.Password.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException(L("FormIsNotValidMessage"));
                    }
                }

                var user = await userRegistrationManager.RegisterAsync(
                    model.FirstName,
                    model.LastName,
                    model.EmailAddress,
                    model.UserName,
                    model.Password,
                    true //Assumed email address is always confirmed. Change this if you want to implement email confirmation.
                );

                //Getting tenant-specific settings
                var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(StudioXZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

                if (model.IsExternalLogin)
                {
                    Debug.Assert(externalLoginInfo != null);
                    
                    if (string.Equals(externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email), model.EmailAddress, StringComparison.OrdinalIgnoreCase))
                    {
                        user.IsEmailConfirmed = true;
                    }

                    user.Logins = new List<UserLogin>
                    {
                        new UserLogin
                        {
                            LoginProvider = externalLoginInfo.LoginProvider,
                            ProviderKey = externalLoginInfo.ProviderKey,
                            TenantId = user.TenantId
                        }
                    };
                }

                await unitOfWorkManager.Current.SaveChangesAsync();

                Debug.Assert(user.TenantId != null);

                var tenant = await tenantManager.GetByIdAsync(user.TenantId.Value);

                //Directly login if possible
                if (user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin))
                {
                    StudioXLoginResult<Tenant, User> loginResult;
                    if (externalLoginInfo != null)
                    {
                        loginResult = await logInManager.LoginAsync(externalLoginInfo, tenant.TenancyName);
                    }
                    else
                    {
                        loginResult = await GetLoginResultAsync(user.UserName, model.Password, tenant.TenancyName);
                    }

                    if (loginResult.Result == StudioXLoginResultType.Success)
                    {
                        await signInManager.SignInAsync(loginResult.Identity, false);
                        return Redirect(GetAppHomeUrl());
                    }

                    Logger.Warn("New registered user could not be login. This should not be normally. login result: " + loginResult.Result);
                }

                return View("RegisterResult", new RegisterResultViewModel
                {
                    TenancyName = tenant.TenancyName,
                    NameAndLastName = user.FirstName + " " + user.LastName,
                    UserName = user.UserName,
                    EmailAddress = user.EmailAddress,
                    IsEmailConfirmed = user.IsEmailConfirmed,
                    IsActive = user.IsActive,
                    IsEmailConfirmationRequiredForLogin = isEmailConfirmationRequiredForLogin
                });
            }
            catch (UserFriendlyException ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                return View("Register", model);
            }
        }

        #endregion

        #region External Login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action(
                "ExternalLoginCallback",
                "Account",
                new
                {
                    ReturnUrl = returnUrl,
                    authSchema = provider
                });

            return Challenge(
                new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties
                {
                    Items = { { "LoginProvider", provider } },
                    RedirectUri = redirectUrl
                },
                provider
            );
        }

        [UnitOfWork]
        public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl, string authSchema, string remoteError = null)
        {
            returnUrl = NormalizeReturnUrl(returnUrl);
            
            if (remoteError != null)
            {
                Logger.Error("Remote Error in ExternalLoginCallback: " + remoteError);
                throw new UserFriendlyException(L("CouldNotCompleteLoginOperation"));
            }

            var externalLoginInfo = await signInManager.GetExternalLoginInfoAsync(authSchema);
            if (externalLoginInfo == null)
            {
                Logger.Warn("Could not get information from external login.");
                return RedirectToAction(nameof(Login));
            }

            await signInManager.SignOutAsync();

            var tenancyName = GetTenancyNameOrNull();

            var loginResult = await logInManager.LoginAsync(externalLoginInfo, tenancyName);

            switch (loginResult.Result)
            {
                case StudioXLoginResultType.Success:
                    await signInManager.SignInAsync(loginResult.Identity, false);
                    return Redirect(returnUrl);
                case StudioXLoginResultType.UnknownExternalLogin:
                    return await RegisterForExternalLogin(externalLoginInfo);
                default:
                    throw studioXLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                        loginResult.Result,
                        externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email) ?? externalLoginInfo.ProviderKey,
                        tenancyName
                    );
            }
        }

        private async Task<ActionResult> RegisterForExternalLogin(ExternalLoginInfo externalLoginInfo)
        {
            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            var nameInfo = ExternalLoginInfoHelper.GetFirstNameAndLastNameFromClaims(externalLoginInfo.Principal.Claims.ToList());

            var viewModel = new RegisterViewModel
            {
                EmailAddress = email,
                FirstName = nameInfo.firstName,
                LastName = nameInfo.lastName,
                IsExternalLogin = true,
                ExternalLoginAuthSchema = externalLoginInfo.LoginProvider
            };

            if (nameInfo.firstName != null &&
                nameInfo.lastName != null &&
                email != null)
            {
                return await Register(viewModel);
            }

            return RegisterView(viewModel);
        }

        [UnitOfWork]
        protected virtual async Task<List<Tenant>> FindPossibleTenantsOfUserAsync(UserLoginInfo login)
        {
            List<User> allUsers;
            using (unitOfWorkManager.Current.DisableFilter(StudioXDataFilters.MayHaveTenant))
            {
                allUsers = await userManager.FindAllAsync(login);
            }

            return allUsers
                .Where(u => u.TenantId != null)
                .Select(u => AsyncHelper.RunSync(() => tenantManager.FindByIdAsync(u.TenantId.Value)))
                .ToList();
        }

        #endregion

        #region Helpers

        public ActionResult RedirectToAppHome()
        {
            return RedirectToAction("Index", "Home");
        }

        public string GetAppHomeUrl()
        {
            return Url.Action("Index", "Home");
        }

        #endregion

        #region Change Tenant

        public async Task<ActionResult> TenantChangeModal()
        {
            var loginInfo = await sessionAppService.GetCurrentLoginInformations();
            return View("/Views/Shared/Components/TenantChange/_ChangeModal.cshtml", new ChangeModalViewModel
            {
                TenancyName = loginInfo.Tenant?.TenancyName
            });
        }

        #endregion

        #region Common

        private string GetTenancyNameOrNull()
        {
            if (!StudioXSession.TenantId.HasValue)
            {
                return null;
            }

            return tenantCache.GetOrNull(StudioXSession.TenantId.Value)?.TenancyName;
        }

        private string NormalizeReturnUrl(string returnUrl, Func<string> defaultValueBuilder = null)
        {
            if (defaultValueBuilder == null)
            {
                defaultValueBuilder = GetAppHomeUrl;
            }

            if (returnUrl.IsNullOrEmpty())
            {
                return defaultValueBuilder();
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return returnUrl;
            }

            return defaultValueBuilder();
        }

        #endregion

        #region Etc

        /// <summary>
        /// This is a demo code to demonstrate sending notification to default tenant admin and host admin uers.
        /// Don't use this code in production !!!
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ActionResult> TestNotification(string message = "")
        {
            if (message.IsNullOrEmpty())
            {
                message = "This is a test notification, created at " + Clock.Now;
            }

            var defaultTenantAdmin = new UserIdentifier(1, 2);
            var hostAdmin = new UserIdentifier(null, 1);

            await notificationPublisher.PublishAsync(
                    "App.SimpleMessage",
                    new MessageNotificationData(message),
                    severity: NotificationSeverity.Info,
                    userIds: new[] { defaultTenantAdmin, hostAdmin }
                 );

            return Content("Sent notification: " + message);
        }

        #endregion
    }
}