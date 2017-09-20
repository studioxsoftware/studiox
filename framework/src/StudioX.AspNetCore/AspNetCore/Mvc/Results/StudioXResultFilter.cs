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
        private readonly IStudioXAspNetCoreConfiguration _configuration;
        private readonly IStudioXActionResultWrapperFactory _actionResultWrapperFactory;

        public StudioXResultFilter(IStudioXAspNetCoreConfiguration configuration, 
            IStudioXActionResultWrapperFactory actionResultWrapper)
        {
            _configuration = configuration;
            _actionResultWrapperFactory = actionResultWrapper;
        }

        public virtual void OnResultExecuting(ResultExecutingContext context)
        {
            if (!context.ActionDescriptor.IsControllerAction())
            {
                return;
            }

            var methodInfo = context.ActionDescriptor.GetMethodInfo();

            //var clientCacheAttribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
            //    methodInfo,
            //    _configuration.DefaultClientCacheAttribute
            //);

            //clientCacheAttribute?.Apply(context);
            
            var wrapResultAttribute =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    methodInfo,
                    _configuration.DefaultWrapResultAttribute
                );

            if (!wrapResultAttribute.WrapOnSuccess)
            {
                return;
            }

            _actionResultWrapperFactory.CreateFor(context).Wrap(context);
        }

        public virtual void OnResultExecuted(ResultExecutedContext context)
        {
            //no action
        }
    }
}
