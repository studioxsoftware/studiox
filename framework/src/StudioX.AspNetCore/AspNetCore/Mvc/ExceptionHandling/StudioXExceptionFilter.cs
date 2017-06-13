using System.Net;
using StudioX.AspNetCore.Configuration;
using StudioX.AspNetCore.Mvc.Extensions;
using StudioX.AspNetCore.Mvc.Results;
using StudioX.Authorization;
using StudioX.Dependency;
using StudioX.Domain.Entities;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Exceptions;
using StudioX.Logging;
using StudioX.Reflection;
using StudioX.Runtime.Validation;
using StudioX.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.ExceptionHandling
{
    public class StudioXExceptionFilter : IExceptionFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        public IEventBus EventBus { get; set; }

        private readonly IErrorInfoBuilder errorInfoBuilder;
        private readonly IStudioXAspNetCoreConfiguration configuration;

        public StudioXExceptionFilter(IErrorInfoBuilder errorInfoBuilder, IStudioXAspNetCoreConfiguration configuration)
        {
            this.errorInfoBuilder = errorInfoBuilder;
            this.configuration = configuration;

            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
        }

        public void OnException(ExceptionContext context)
        {
            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    context.ActionDescriptor.GetMethodInfo(),
                    configuration.DefaultWrapResultAttribute
                );

            if (wrapResultAttribute.LogError)
            {
                LogHelper.LogException(Logger, context.Exception);
            }

            if (wrapResultAttribute.WrapOnError)
            {
                HandleAndWrapException(context);
            }
        }

        private void HandleAndWrapException(ExceptionContext context)
        {
            if (!ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
            {
                return;
            }

            context.HttpContext.Response.StatusCode = GetStatusCode(context);

            context.Result = new ObjectResult(
                new AjaxResponse(
                    errorInfoBuilder.BuildForException(context.Exception),
                    context.Exception is StudioXAuthorizationException
                )
            );

            EventBus.Trigger(this, new StudioXHandledExceptionData(context.Exception));

            context.Exception = null; //Handled!
        }

        private int GetStatusCode(ExceptionContext context)
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
    }
}
