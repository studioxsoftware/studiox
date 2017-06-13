using System;
using System.Linq;
using System.Threading.Tasks;
using StudioX.AspNetCore.Mvc.Extensions;
using StudioX.AspNetCore.Mvc.Results;
using StudioX.Authorization;
using StudioX.Dependency;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Exceptions;
using StudioX.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Authorization
{
    public class StudioXAuthorizationFilter : IAsyncAuthorizationFilter, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IAuthorizationHelper authorizationHelper;
        private readonly IErrorInfoBuilder errorInfoBuilder;
        private readonly IEventBus eventBus;

        public StudioXAuthorizationFilter(
            IAuthorizationHelper authorizationHelper,
            IErrorInfoBuilder errorInfoBuilder,
            IEventBus eventBus)
        {
            this.authorizationHelper = authorizationHelper;
            this.errorInfoBuilder = errorInfoBuilder;
            this.eventBus = eventBus;
            Logger = NullLogger.Instance;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Allow Anonymous skips all authorization
            if (context.Filters.Any(item => item is IAllowAnonymousFilter))
            {
                return;
            }

            try
            {
                //TODO: Avoid using try/catch, use conditional checking
                await authorizationHelper.AuthorizeAsync(context.ActionDescriptor.GetMethodInfo());
            }
            catch (StudioXAuthorizationException ex)
            {
                Logger.Warn(ex.ToString(), ex);

                eventBus.Trigger(this, new StudioXHandledExceptionData(ex));

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    context.Result = new ObjectResult(new AjaxResponse(errorInfoBuilder.BuildForException(ex), true))
                    {
                        StatusCode = context.HttpContext.User.Identity.IsAuthenticated
                            ? (int) System.Net.HttpStatusCode.Forbidden
                            : (int) System.Net.HttpStatusCode.Unauthorized
                    };
                }
                else
                {
                    context.Result = new ChallengeResult();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);

                eventBus.Trigger(this, new StudioXHandledExceptionData(ex));

                if (ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
                {
                    context.Result = new ObjectResult(new AjaxResponse(errorInfoBuilder.BuildForException(ex)))
                    {
                        StatusCode = (int) System.Net.HttpStatusCode.InternalServerError
                    };
                }
                else
                {
                    //TODO: How to return Error page?
                    context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.InternalServerError);
                }
            }
        }
    }
}