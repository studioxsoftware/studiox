using System.Net;
using System.Reflection;
using System.Web.Mvc;
using StudioX.Authorization;
using StudioX.Dependency;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Exceptions;
using StudioX.Logging;
using StudioX.Web.Models;
using StudioX.Web.Mvc.Controllers.Results;
using StudioX.Web.Mvc.Extensions;
using StudioX.Web.Mvc.Helpers;

namespace StudioX.Web.Mvc.Authorization
{
    public class StudioXMvcAuthorizeFilter : IAuthorizationFilter, ITransientDependency
    {
        private readonly IAuthorizationHelper _authorizationHelper;
        private readonly IErrorInfoBuilder _errorInfoBuilder;
        private readonly IEventBus _eventBus;

        public StudioXMvcAuthorizeFilter(
            IAuthorizationHelper authorizationHelper,
            IErrorInfoBuilder errorInfoBuilder,
            IEventBus eventBus)
        {
            _authorizationHelper = authorizationHelper;
            _errorInfoBuilder = errorInfoBuilder;
            _eventBus = eventBus;
        }

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                return;
            }

            var methodInfo = filterContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return;
            }

            try
            {
                _authorizationHelper.Authorize(methodInfo, methodInfo.DeclaringType);
            }
            catch (StudioXAuthorizationException ex)
            {
                LogHelper.Logger.Warn(ex.ToString(), ex);
                HandleUnauthorizedRequest(filterContext, methodInfo, ex);
            }
        }

        protected virtual void HandleUnauthorizedRequest(
            AuthorizationContext filterContext,
            MethodInfo methodInfo,
            StudioXAuthorizationException ex)
        {
            filterContext.HttpContext.Response.StatusCode =
                filterContext.RequestContext.HttpContext.User?.Identity?.IsAuthenticated ?? false
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;

            var isJsonResult = MethodInfoHelper.IsJsonResult(methodInfo);

            if (isJsonResult)
            {
                filterContext.Result = CreateUnAuthorizedJsonResult(ex);
            }
            else
            {
                filterContext.Result = CreateUnAuthorizedNonJsonResult(filterContext, ex);
            }

            if (isJsonResult || filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
            }

            _eventBus.Trigger(this, new StudioXHandledExceptionData(ex));
        }

        protected virtual StudioXJsonResult CreateUnAuthorizedJsonResult(StudioXAuthorizationException ex)
        {
            return new StudioXJsonResult(
                new AjaxResponse(_errorInfoBuilder.BuildForException(ex), true))
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        protected virtual HttpStatusCodeResult CreateUnAuthorizedNonJsonResult(AuthorizationContext filterContext, StudioXAuthorizationException ex)
        {
            return new HttpStatusCodeResult(filterContext.HttpContext.Response.StatusCode, ex.Message);
        }
    }
}