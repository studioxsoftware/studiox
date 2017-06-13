using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Aspects;
using StudioX.AspNetCore.Configuration;
using StudioX.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Validation
{
    public class StudioXValidationActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IIocResolver iocResolver;
        private readonly IStudioXAspNetCoreConfiguration configuration;

        public StudioXValidationActionFilter(IIocResolver iocResolver, IStudioXAspNetCoreConfiguration configuration)
        {
            this.iocResolver = iocResolver;
            this.configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!configuration.IsValidationEnabledForControllers)
            {
                await next();
                return;
            }

            using (StudioXCrossCuttingConcerns.Applying(context.Controller, StudioXCrossCuttingConcerns.Validation))
            {
                using (var validator = iocResolver.ResolveAsDisposable<MvcActionInvocationValidator>())
                {
                    validator.Object.Initialize(context);
                    validator.Object.Validate();
                }

                await next();
            }
        }
    }
}
