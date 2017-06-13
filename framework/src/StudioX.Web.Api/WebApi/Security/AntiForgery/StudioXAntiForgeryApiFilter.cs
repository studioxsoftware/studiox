using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using StudioX.Dependency;
using StudioX.Web.Security.AntiForgery;
using StudioX.WebApi.Configuration;
using StudioX.WebApi.Controllers.Dynamic.Selectors;
using StudioX.WebApi.Validation;
using Castle.Core.Logging;

namespace StudioX.WebApi.Security.AntiForgery
{
    public class StudioXAntiForgeryApiFilter : IAuthorizationFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public bool AllowMultiple => false;

        private readonly IStudioXAntiForgeryManager studioXAntiForgeryManager;
        private readonly IStudioXWebApiConfiguration webApiConfiguration;
        private readonly IStudioXAntiForgeryWebConfiguration antiForgeryWebConfiguration;

        public StudioXAntiForgeryApiFilter(
            IStudioXAntiForgeryManager antiForgeryManager, 
            IStudioXWebApiConfiguration webApiConfiguration,
            IStudioXAntiForgeryWebConfiguration antiForgeryWebConfiguration)
        {
            studioXAntiForgeryManager = antiForgeryManager;
            this.webApiConfiguration = webApiConfiguration;
            this.antiForgeryWebConfiguration = antiForgeryWebConfiguration;
            Logger = NullLogger.Instance;
        }

        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            var methodInfo = actionContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return await continuation();
            }

            if (!studioXAntiForgeryManager.ShouldValidate(antiForgeryWebConfiguration, methodInfo, actionContext.Request.Method.ToHttpVerb(), webApiConfiguration.IsAutomaticAntiForgeryValidationEnabled))
            {
                return await continuation();
            }

            if (!studioXAntiForgeryManager.IsValid(actionContext.Request.Headers))
            {
                return CreateErrorResponse(actionContext, "Empty or invalid anti forgery header token.");
            }

            return await continuation();
        }

        protected virtual HttpResponseMessage CreateErrorResponse(HttpActionContext actionContext, string reason)
        {
            Logger.Warn(reason);
            Logger.Warn("Requested URI: " + actionContext.Request.RequestUri);
            return new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = reason };
        }
    }
}