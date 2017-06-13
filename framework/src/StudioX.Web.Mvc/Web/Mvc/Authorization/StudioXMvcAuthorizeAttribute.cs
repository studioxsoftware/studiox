using System;
using System.Web.Mvc;
using StudioX.Authorization;

namespace StudioX.Web.Mvc.Authorization
{
    /// <summary>
    /// This attribute is used on an action of an MVC <see cref="Controller"/>
    /// to make that action usable only by authorized users. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class StudioXMvcAuthorizeAttribute : AuthorizeAttribute, IStudioXAuthorizeAttribute
    {
        /// <inheritdoc/>
        public string[] Permissions { get; set; }

        /// <inheritdoc/>
        public bool RequireAllPermissions { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="StudioXMvcAuthorizeAttribute"/> class.
        /// </summary>
        /// <param name="permissions">A list of permissions to authorize</param>
        public StudioXMvcAuthorizeAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            if (!httpContext.Request.IsAjaxRequest())
            {
                base.HandleUnauthorizedRequest(filterContext);
                return;
            }

            httpContext.Response.StatusCode = httpContext.User.Identity.IsAuthenticated == false
                                      ? (int)System.Net.HttpStatusCode.Unauthorized
                                      : (int)System.Net.HttpStatusCode.Forbidden;

            httpContext.Response.SuppressFormsAuthenticationRedirect = true;
            httpContext.Response.End();
        }
    }
}