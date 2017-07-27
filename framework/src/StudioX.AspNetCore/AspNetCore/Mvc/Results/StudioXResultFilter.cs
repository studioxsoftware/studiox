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
        private readonly IStudioXActionResultWrapperFactory actionResultWrapperFactory;

        public StudioXResultFilter(IStudioXAspNetCoreConfiguration configuration, 
            IStudioXActionResultWrapperFactory actionResultWrapperFactory)
        {
            this.configuration = configuration;
            this.actionResultWrapperFactory = actionResultWrapperFactory;
        }

        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
            var methodInfo = context.ActionDescriptor.GetMethodInfo();

            var cacheResultAttribute =
            ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                methodInfo,
                configuration.DefaultCacheResultAttribute
            );

            if (!cacheResultAttribute.NoCache)
            {
                SetCache(context,
                    cacheResultAttribute.MustRevalidate,
                    cacheResultAttribute.PrivateOnly,
                    cacheResultAttribute.MaxAge);
            }
            else if (cacheResultAttribute.NoCache ||
                (configuration.SetNoCacheForAjaxResponses && context.HttpContext.Request.IsAjaxRequest()))
            {
                SetNoCache(context);
            }

            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    methodInfo,
                    configuration.DefaultWrapResultAttribute
                );

            if (!wrapResultAttribute.WrapOnSuccess)
            {
                return;
            }

            actionResultWrapperFactory.CreateFor(context).Wrap(context);
        }


        private void SetCache(ResultExecutingContext context, bool mustRevalidate, bool privateOnly, int maxAge)
        {
            if (maxAge > 0)
            {
                context.HttpContext.Response.Headers["Cache-Control"] =
                    (privateOnly ? "private, " : "public, ") +
                    (mustRevalidate ? "must-revalidate, " : "") +
                    ("max-age=" + maxAge);
            }
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
