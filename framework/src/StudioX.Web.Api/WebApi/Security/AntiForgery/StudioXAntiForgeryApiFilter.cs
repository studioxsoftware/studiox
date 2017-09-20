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

        private readonly IStudioXAntiForgeryManager _studioXAntiForgeryManager;
        private readonly IStudioXWebApiConfiguration _webApiConfiguration;
        private readonly IStudioXAntiForgeryWebConfiguration _antiForgeryWebConfiguration;

        public StudioXAntiForgeryApiFilter(
            IStudioXAntiForgeryManager studioxAntiForgeryManager, 
            IStudioXWebApiConfiguration webApiConfiguration,
            IStudioXAntiForgeryWebConfiguration antiForgeryWebConfiguration)
        {
            _studioXAntiForgeryManager = studioxAntiForgeryManager;
            _webApiConfiguration = webApiConfiguration;
            _antiForgeryWebConfiguration = antiForgeryWebConfiguration;
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

            if (!_studioXAntiForgeryManager.ShouldValidate(_antiForgeryWebConfiguration, methodInfo, actionContext.Request.Method.ToHttpVerb(), _webApiConfiguration.IsAutomaticAntiForgeryValidationEnabled))
            {
                return await continuation();
            }

            if (!_studioXAntiForgeryManager.IsValid(actionContext.Request.Headers))
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