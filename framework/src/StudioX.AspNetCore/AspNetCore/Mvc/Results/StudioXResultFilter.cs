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

        public virtual void OnResultExecuted(ResultExecutedContext context)
        {
            //no action
        }
    }
}