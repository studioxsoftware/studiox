using System.Net;
using System.Reflection;
using System.Web.Mvc;
using StudioX.Dependency;
using StudioX.Web.Models;
using StudioX.Web.Mvc.Configuration;
using StudioX.Web.Mvc.Controllers.Results;
using StudioX.Web.Mvc.Extensions;
using StudioX.Web.Mvc.Helpers;
using StudioX.Web.Security.AntiForgery;
using Castle.Core.Logging;

namespace StudioX.Web.Mvc.Security.AntiForgery
{
    public class StudioXAntiForgeryMvcFilter: IAuthorizationFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IStudioXAntiForgeryManager _studioXAntiForgeryManager;
        private readonly IStudioXMvcConfiguration _mvcConfiguration;
        private readonly IStudioXAntiForgeryWebConfiguration _antiForgeryWebConfiguration;

        public StudioXAntiForgeryMvcFilter(
            IStudioXAntiForgeryManager studioxAntiForgeryManager, 
            IStudioXMvcConfiguration mvcConfiguration,
            IStudioXAntiForgeryWebConfiguration antiForgeryWebConfiguration)
        {
            _studioXAntiForgeryManager = studioxAntiForgeryManager;
            _mvcConfiguration = mvcConfiguration;
            _antiForgeryWebConfiguration = antiForgeryWebConfiguration;
            Logger = NullLogger.Instance;
        }

        public void OnAuthorization(AuthorizationContext context)
        {
            var methodInfo = context.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return;
            }

            var httpVerb = HttpVerbHelper.Create(context.HttpContext.Request.HttpMethod);
            if (!_studioXAntiForgeryManager.ShouldValidate(_antiForgeryWebConfiguration, methodInfo, httpVerb, _mvcConfiguration.IsAutomaticAntiForgeryValidationEnabled))
            {
                return;
            }

            if (!_studioXAntiForgeryManager.IsValid(context.HttpContext))
            {
                CreateErrorResponse(context, methodInfo, "Empty or invalid anti forgery header token.");
            }
        }

        private void CreateErrorResponse(
            AuthorizationContext context, 
            MethodInfo methodInfo, 
            string message)
        {
            Logger.Warn(message);
            Logger.Warn("Requested URI: " + context.HttpContext.Request.Url);

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.HttpContext.Response.StatusDescription = message;

            var isJsonResult = MethodInfoHelper.IsJsonResult(methodInfo);

            if (isJsonResult)
            {
                context.Result = CreateUnAuthorizedJsonResult(message);
            }
            else
            {
                context.Result = CreateUnAuthorizedNonJsonResult(context, message);
            }

            if (isJsonResult || context.HttpContext.Request.IsAjaxRequest())
            {
                context.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
            }
        }

        protected virtual StudioXJsonResult CreateUnAuthorizedJsonResult(string message)
        {
            return new StudioXJsonResult(new AjaxResponse(new ErrorInfo(message), true))
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        protected virtual HttpStatusCodeResult CreateUnAuthorizedNonJsonResult(AuthorizationContext filterContext, string message)
        {
            return new HttpStatusCodeResult(filterContext.HttpContext.Response.StatusCode, message);
        }
    }
}
