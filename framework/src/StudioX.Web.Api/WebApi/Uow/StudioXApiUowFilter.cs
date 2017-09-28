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
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IStudioXWebApiConfiguration _webApiConfiguration;
        private readonly IUnitOfWorkDefaultOptions _unitOfWorkDefaultOptions;

        public bool AllowMultiple => false;

        public StudioXApiUowFilter(
            IUnitOfWorkManager unitOfWorkManager,
            IStudioXWebApiConfiguration webApiConfiguration, 
            IUnitOfWorkDefaultOptions unitOfWorkDefaultOptions)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _webApiConfiguration = webApiConfiguration;
            _unitOfWorkDefaultOptions = unitOfWorkDefaultOptions;
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

            var unitOfWorkAttr = _unitOfWorkDefaultOptions.GetUnitOfWorkAttributeOrNull(methodInfo) ??
                                 _webApiConfiguration.DefaultUnitOfWorkAttribute;

            if (unitOfWorkAttr.IsDisabled)
            {
                return await continuation();
            }

            using (var uow = _unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
            {
                var result = await continuation();
                await uow.CompleteAsync();
                return result;
            }
        }
    }
}