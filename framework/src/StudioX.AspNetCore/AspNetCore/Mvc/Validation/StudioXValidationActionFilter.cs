using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Aspects;
using StudioX.AspNetCore.Configuration;
using StudioX.AspNetCore.Mvc.Extensions;
using StudioX.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Validation
{
    public class StudioXValidationActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly IStudioXAspNetCoreConfiguration _configuration;

        public StudioXValidationActionFilter(IIocResolver iocResolver, IStudioXAspNetCoreConfiguration configuration)
        {
            _iocResolver = iocResolver;
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!_configuration.IsValidationEnabledForControllers || !context.ActionDescriptor.IsControllerAction())
            {
                await next();
                return;
            }

            using (StudioXCrossCuttingConcerns.Applying(context.Controller, StudioXCrossCuttingConcerns.Validation))
            {
                using (var validator = _iocResolver.ResolveAsDisposable<MvcActionInvocationValidator>())
                {
                    validator.Object.Initialize(context);
                    validator.Object.Validate();
                }

                await next();
            }
        }
    }
}
