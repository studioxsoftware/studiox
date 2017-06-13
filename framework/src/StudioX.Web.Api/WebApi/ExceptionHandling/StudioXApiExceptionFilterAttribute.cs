using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using StudioX.Dependency;
using StudioX.Domain.Entities;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Exceptions;
using StudioX.Extensions;
using StudioX.Logging;
using StudioX.Runtime.Session;
using StudioX.Runtime.Validation;
using StudioX.Web.Models;
using StudioX.WebApi.Configuration;
using StudioX.WebApi.Controllers;
using Castle.Core.Logging;

namespace StudioX.WebApi.ExceptionHandling
{
    /// <summary>
    /// Used to handle exceptions on web api controllers.
    /// </summary>
    public class StudioXApiExceptionFilterAttribute : ExceptionFilterAttribute, ITransientDependency
    {
        /// <summary>
        /// Reference to the <see cref="ILogger"/>.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Reference to the <see cref="IEventBus"/>.
        /// </summary>
        public IEventBus EventBus { get; set; }

        public IStudioXSession StudioXSession { get; set; }

        protected IStudioXWebApiConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudioXApiExceptionFilterAttribute"/> class.
        /// </summary>
        public StudioXApiExceptionFilterAttribute(IStudioXWebApiConfiguration configuration)
        {
            Configuration = configuration;
            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
            StudioXSession = NullStudioXSession.Instance;
        }

        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            var wrapResultAttribute = HttpActionDescriptorHelper
                .GetWrapResultAttributeOrNull(context.ActionContext.ActionDescriptor) ??
                Configuration.DefaultWrapResultAttribute;

            if (wrapResultAttribute.LogError)
            {
                LogHelper.LogException(Logger, context.Exception);
            }

            if (!wrapResultAttribute.WrapOnError)
            {
                return;
            }

            if (IsIgnoredUrl(context.Request.RequestUri))
            {
                return;
            }

            if (context.Exception is HttpException)
            {
                var httpException = context.Exception as HttpException;
                var httpStatusCode = (HttpStatusCode) httpException.GetHttpCode();

                context.Response = context.Request.CreateResponse(
                    httpStatusCode,
                    new AjaxResponse(
                        new ErrorInfo(httpException.Message),
                        httpStatusCode == HttpStatusCode.Unauthorized || httpStatusCode == HttpStatusCode.Forbidden
                    )
                );
            }
            else
            {
                context.Response = context.Request.CreateResponse(
                    GetStatusCode(context),
                    new AjaxResponse(
                        SingletonDependency<ErrorInfoBuilder>.Instance.BuildForException(context.Exception),
                        context.Exception is StudioX.Authorization.StudioXAuthorizationException)
                );
            }

            EventBus.Trigger(this, new StudioXHandledExceptionData(context.Exception));
        }

        protected virtual HttpStatusCode GetStatusCode(HttpActionExecutedContext context)
        {
            if (context.Exception is StudioX.Authorization.StudioXAuthorizationException)
            {
                return StudioXSession.UserId.HasValue
                    ? HttpStatusCode.Forbidden
                    : HttpStatusCode.Unauthorized;
            }

            if (context.Exception is StudioXValidationException)
            {
                return HttpStatusCode.BadRequest;
            }

            if (context.Exception is EntityNotFoundException)
            {
                return HttpStatusCode.NotFound;
            }

            return HttpStatusCode.InternalServerError;
        }

        protected virtual bool IsIgnoredUrl(Uri uri)
        {
            if (uri == null || uri.AbsolutePath.IsNullOrEmpty())
            {
                return false;
            }

            return Configuration.ResultWrappingIgnoreUrls.Any(url => uri.AbsolutePath.StartsWith(url));
        }
    }
}