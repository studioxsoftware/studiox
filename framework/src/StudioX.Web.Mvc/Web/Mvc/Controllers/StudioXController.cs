using System;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using StudioX.Application.Features;
using StudioX.Authorization;
using StudioX.Configuration;
using StudioX.Domain.Entities;
using StudioX.Domain.Uow;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Exceptions;
using StudioX.Localization;
using StudioX.Localization.Sources;
using StudioX.Logging;
using StudioX.ObjectMapping;
using StudioX.Reflection;
using StudioX.Runtime.Session;
using StudioX.Runtime.Validation;
using StudioX.Web.Models;
using StudioX.Web.Mvc.Configuration;
using StudioX.Web.Mvc.Controllers.Results;
using StudioX.Web.Mvc.Extensions;
using StudioX.Web.Mvc.Helpers;
using StudioX.Web.Mvc.Models;
using Castle.Core.Logging;

namespace StudioX.Web.Mvc.Controllers
{
    /// <summary>
    /// Base class for all MVC Controllers in StudioX system.
    /// </summary>
    public abstract class StudioXController : Controller
    {
        /// <summary>
        /// Gets current session information.
        /// </summary>
        public IStudioXSession StudioXSession { get; set; }

        /// <summary>
        /// Gets the event bus.
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        /// Reference to the permission manager.
        /// </summary>
        public IPermissionManager PermissionManager { get; set; }

        /// <summary>
        /// Reference to the setting manager.
        /// </summary>
        public ISettingManager SettingManager { get; set; }

        /// <summary>
        /// Reference to the permission checker.
        /// </summary>
        public IPermissionChecker PermissionChecker { protected get; set; }

        /// <summary>
        /// Reference to the feature manager.
        /// </summary>
        public IFeatureManager FeatureManager { protected get; set; }

        /// <summary>
        /// Reference to the permission checker.
        /// </summary>
        public IFeatureChecker FeatureChecker { protected get; set; }

        /// <summary>
        /// Reference to the localization manager.
        /// </summary>
        public ILocalizationManager LocalizationManager { protected get; set; }

        /// <summary>
        /// Reference to the error info builder.
        /// </summary>
        public IErrorInfoBuilder ErrorInfoBuilder { protected get; set; }

        /// <summary>
        /// Gets/sets name of the localization source that is used in this application service.
        /// It must be set in order to use <see cref="L(string)"/> and <see cref="L(string,CultureInfo)"/> methods.
        /// </summary>
        protected string LocalizationSourceName { get; set; }

        /// <summary>
        /// Gets localization source.
        /// It's valid if <see cref="LocalizationSourceName"/> is set.
        /// </summary>
        protected ILocalizationSource LocalizationSource
        {
            get
            {
                if (LocalizationSourceName == null)
                {
                    throw new StudioXException("Must set LocalizationSourceName before, in order to get LocalizationSource");
                }

                if (localizationSource == null || localizationSource.Name != LocalizationSourceName)
                {
                    localizationSource = LocalizationManager.GetSource(LocalizationSourceName);
                }

                return localizationSource;
            }
        }
        private ILocalizationSource localizationSource;

        /// <summary>
        /// Reference to the logger to write logs.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Reference to the object to object mapper.
        /// </summary>
        public IObjectMapper ObjectMapper { get; set; }

        /// <summary>
        /// Reference to <see cref="IUnitOfWorkManager"/>.
        /// </summary>
        public IUnitOfWorkManager UnitOfWorkManager
        {
            get
            {
                if (unitOfWorkManager == null)
                {
                    throw new StudioXException("Must set UnitOfWorkManager before use it.");
                }

                return unitOfWorkManager;
            }
            set => unitOfWorkManager = value;
        }
        private IUnitOfWorkManager unitOfWorkManager;

        /// <summary>
        /// Gets current unit of work.
        /// </summary>
        protected IActiveUnitOfWork CurrentUnitOfWork => UnitOfWorkManager.Current;

        public IStudioXMvcConfiguration StudioXMvcConfiguration { get; set; }

        /// <summary>
        /// MethodInfo for currently executing action.
        /// </summary>
        private MethodInfo currentMethodInfo;

        /// <summary>
        /// WrapResultAttribute for currently executing action.
        /// </summary>
        private WrapResultAttribute wrapResultAttribute;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected StudioXController()
        {
            StudioXSession = NullStudioXSession.Instance;
            Logger = NullLogger.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
            PermissionChecker = NullPermissionChecker.Instance;
            EventBus = NullEventBus.Instance;
            ObjectMapper = NullObjectMapper.Instance;
        }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name)
        {
            return LocalizationSource.GetString(name);
        }

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected string L(string name, params object[] args)
        {
            return LocalizationSource.GetString(name, args);
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        protected virtual string L(string name, CultureInfo culture)
        {
            return LocalizationSource.GetString(name, culture);
        }

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        protected string L(string name, CultureInfo culture, params object[] args)
        {
            return LocalizationSource.GetString(name, culture, args);
        }

        /// <summary>
        /// Checks if current user is granted for a permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission</param>
        protected Task<bool> IsGrantedAsync(string permissionName)
        {
            return PermissionChecker.IsGrantedAsync(permissionName);
        }

        /// <summary>
        /// Checks if current user is granted for a permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission</param>
        protected bool IsGranted(string permissionName)
        {
            return PermissionChecker.IsGranted(permissionName);
        }


        /// <summary>
        /// Checks if given feature is enabled for current tenant.
        /// </summary>
        /// <param name="featureName">Name of the feature</param>
        /// <returns></returns>
        protected virtual Task<bool> IsEnabledAsync(string featureName)
        {
            return FeatureChecker.IsEnabledAsync(featureName);
        }

        /// <summary>
        /// Checks if given feature is enabled for current tenant.
        /// </summary>
        /// <param name="featureName">Name of the feature</param>
        /// <returns></returns>
        protected virtual bool IsEnabled(string featureName)
        {
            return FeatureChecker.IsEnabled(featureName);
        }

        /// <summary>
        /// Json the specified data, contentType, contentEncoding and behavior.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="contentType">Content type.</param>
        /// <param name="contentEncoding">Content encoding.</param>
        /// <param name="behavior">Behavior.</param>
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            if (wrapResultAttribute != null && !wrapResultAttribute.WrapOnSuccess)
            {
                return base.Json(data, contentType, contentEncoding, behavior);
            }

            return StudioXJson(data, contentType, contentEncoding, behavior);
        }

        protected virtual StudioXJsonResult StudioXJson(
            object data,
            string contentType = null,
            Encoding contentEncoding = null,
            JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet,
            bool wrapResult = true,
            bool camelCase = true,
            bool indented = false)
        {
            if (wrapResult)
            {
                if (data == null)
                {
                    data = new AjaxResponse();
                }
                else if (!(data is AjaxResponseBase))
                {
                    data = new AjaxResponse(data);
                }
            }

            return new StudioXJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                CamelCase = camelCase,
                Indented = indented
            };
        }

        #region OnActionExecuting / OnActionExecuted

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SetCurrentMethodInfoAndWrapResultAttribute(filterContext);
            base.OnActionExecuting(filterContext);
        }

        private void SetCurrentMethodInfoAndWrapResultAttribute(ActionExecutingContext filterContext)
        {
            //Prevent overriding for child actions
            if (currentMethodInfo != null)
            {
                return;
            }

            currentMethodInfo = filterContext.ActionDescriptor.GetMethodInfoOrNull();
            wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    currentMethodInfo,
                    StudioXMvcConfiguration.DefaultWrapResultAttribute
                );
        }

        #endregion

        #region Exception handling

        protected override void OnException(ExceptionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            //If exception handled before, do nothing.
            //If this is child action, exception should be handled by main action.
            if (context.ExceptionHandled || context.IsChildAction)
            {
                base.OnException(context);
                return;
            }

            //Log exception
            if (wrapResultAttribute == null || wrapResultAttribute.LogError)
            {
                LogHelper.LogException(Logger, context.Exception);
            }

            // If custom errors are disabled, we need to let the normal ASP.NET exception handler
            // execute so that the user can see useful debugging information.
            if (!context.HttpContext.IsCustomErrorEnabled)
            {
                base.OnException(context);
                return;
            }

            // If this is not an HTTP 500 (for example, if somebody throws an HTTP 404 from an action method),
            // ignore it.
            if (new HttpException(null, context.Exception).GetHttpCode() != 500)
            {
                base.OnException(context);
                return;
            }

            //Check WrapResultAttribute
            if (wrapResultAttribute == null || !wrapResultAttribute.WrapOnError)
            {
                base.OnException(context);
                return;
            }

            //We handled the exception!
            context.ExceptionHandled = true;

            //Return an error response to the client.
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.StatusCode = GetStatusCodeForException(context);

            context.Result = MethodInfoHelper.IsJsonResult(currentMethodInfo)
                ? GenerateJsonExceptionResult(context)
                : GenerateNonJsonExceptionResult(context);

            // Certain versions of IIS will sometimes use their own error page when
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            context.HttpContext.Response.TrySkipIisCustomErrors = true;

            //Trigger an event, so we can register it.
            EventBus.Trigger(this, new StudioXHandledExceptionData(context.Exception));
        }

        protected virtual int GetStatusCodeForException(ExceptionContext context)
        {
            if (context.Exception is StudioXAuthorizationException)
            {
                return context.HttpContext.User.Identity.IsAuthenticated
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;
            }

            if (context.Exception is StudioXValidationException)
            {
                return (int)HttpStatusCode.BadRequest;
            }

            if (context.Exception is EntityNotFoundException)
            {
                return (int)HttpStatusCode.NotFound;
            }

            return (int)HttpStatusCode.InternalServerError;
        }

        protected virtual ActionResult GenerateJsonExceptionResult(ExceptionContext context)
        {
            context.HttpContext.Items.Add("IgnoreJsonRequestBehaviorDenyGet", "true");
            return new StudioXJsonResult(
                new AjaxResponse(
                    ErrorInfoBuilder.BuildForException(context.Exception),
                    context.Exception is StudioXAuthorizationException
                    )
                );
        }

        protected virtual ActionResult GenerateNonJsonExceptionResult(ExceptionContext context)
        {
            return new ViewResult
            {
                ViewName = "Error",
                MasterName = string.Empty,
                ViewData = new ViewDataDictionary<ErrorViewModel>(new ErrorViewModel(ErrorInfoBuilder.BuildForException(context.Exception), context.Exception)),
                TempData = context.Controller.TempData
            };
        }

        #endregion
    }
}