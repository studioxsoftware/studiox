using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.WebApi.Configuration;
using StudioX.WebApi.Validation;

namespace StudioX.WebApi.Uow
{
    public class StudioXApiUowFilter : IActionFilter, ITransientDependency
    {
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly IStudioXWebApiConfiguration webApiConfiguration;
        private readonly IUnitOfWorkDefaultOptions unitOfWorkDefaultOptions;

        public bool AllowMultiple => false;

        public StudioXApiUowFilter(
            IUnitOfWorkManager unitOfWorkManager,
            IStudioXWebApiConfiguration webApiConfiguration, 
            IUnitOfWorkDefaultOptions unitOfWorkDefaultOptions)
        {
            this.unitOfWorkManager = unitOfWorkManager;
            this.webApiConfiguration = webApiConfiguration;
            this.unitOfWorkDefaultOptions = unitOfWorkDefaultOptions;
        }

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var methodInfo = actionContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return await continuation();
            }

            if (actionContext.ActionDescriptor.IsDynamicStudioXAction())
            {
                return await continuation();
            }

            var unitOfWorkAttr = unitOfWorkDefaultOptions.GetUnitOfWorkAttributeOrNull(methodInfo) ??
                                 webApiConfiguration.DefaultUnitOfWorkAttribute;

            if (unitOfWorkAttr.IsDisabled)
            {
                return await continuation();
            }

            using (var uow = unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
            {
                var result = await continuation();
                await uow.CompleteAsync();
                return result;
            }
        }
    }
}