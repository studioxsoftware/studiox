using System.Threading.Tasks;
using StudioX.AspNetCore.Configuration;
using StudioX.AspNetCore.Mvc.Extensions;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Uow
{
    public class StudioXUowActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly IStudioXAspNetCoreConfiguration aspnetCoreConfiguration;
        private readonly IUnitOfWorkDefaultOptions unitOfWorkDefaultOptions;

        public StudioXUowActionFilter(
            IUnitOfWorkManager unitOfWorkManager,
            IStudioXAspNetCoreConfiguration aspnetCoreConfiguration,
            IUnitOfWorkDefaultOptions unitOfWorkDefaultOptions)
        {
            this.unitOfWorkManager = unitOfWorkManager;
            this.aspnetCoreConfiguration = aspnetCoreConfiguration;
            this.unitOfWorkDefaultOptions = unitOfWorkDefaultOptions;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var unitOfWorkAttr = unitOfWorkDefaultOptions
                .GetUnitOfWorkAttributeOrNull(context.ActionDescriptor.GetMethodInfo()) ??
                aspnetCoreConfiguration.DefaultUnitOfWorkAttribute;

            if (unitOfWorkAttr.IsDisabled)
            {
                await next();
                return;
            }

            using (var uow = unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
            {
                var result = await next();
                if (result.Exception == null || result.ExceptionHandled)
                {
                    await uow.CompleteAsync();
                }
            }
        }
    }
}
