using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using StudioX.Dependency;
using StudioX.WebApi.Configuration;

namespace StudioX.WebApi.Validation
{
    public class StudioXApiValidationFilter : IActionFilter, ITransientDependency
    {
        public bool AllowMultiple => false;

        private readonly IIocResolver iocResolver;
        private readonly IStudioXWebApiConfiguration configuration;

        public StudioXApiValidationFilter(IIocResolver iocResolver, IStudioXWebApiConfiguration configuration)
        {
            this.iocResolver = iocResolver;
            this.configuration = configuration;
        }

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext,
            CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (!configuration.IsValidationEnabledForControllers)
            {
                return await continuation();
            }

            var methodInfo = actionContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return await continuation();
            }

            /* ModelState.IsValid is being checked to handle parameter binding errors (ex: send string for an int value).
             * These type of errors can not be catched from application layer. */

            if (actionContext.ModelState.IsValid
                && actionContext.ActionDescriptor.IsDynamicStudioXAction())
            {
                return await continuation();
            }

            using (var validator = iocResolver.ResolveAsDisposable<WebApiActionInvocationValidator>())
            {
                validator.Object.Initialize(actionContext, methodInfo);
                validator.Object.Validate();
            }

            return await continuation();
        }
    }
}