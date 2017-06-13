using StudioX.AspNetCore.Configuration;
using StudioX.AspNetCore.Mvc.Extensions;
using StudioX.AspNetCore.Mvc.Results.Wrapping;
using StudioX.Dependency;
using StudioX.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Results
{
    public class StudioXResultFilter : IResultFilter, ITransientDependency
    {
        private readonly IStudioXAspNetCoreConfiguration configuration;

        public StudioXResultFilter(IStudioXAspNetCoreConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
            if (configuration.SetNoCacheForAjaxResponses && context.HttpContext.Request.IsAjaxRequest())
            {
                SetNoCache(context);
            }

            var methodInfo = context.ActionDescriptor.GetMethodInfo();
            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    methodInfo,
                    configuration.DefaultWrapResultAttribute
                );

            if (!wrapResultAttribute.WrapOnSuccess)
            {
                return;
            }

            StudioXActionResultWrapperFactory
                .CreateFor(context)
                .Wrap(context);
        }

        public virtual void OnResultExecuted(ResultExecutedContext context)
        {
            //no action
        }
        
        protected virtual void SetNoCache(ResultExecutingContext context)
        {
            //Based on http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, max-age=0";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            context.HttpContext.Response.Headers["Expires"] = "0";
        }
    }
}
