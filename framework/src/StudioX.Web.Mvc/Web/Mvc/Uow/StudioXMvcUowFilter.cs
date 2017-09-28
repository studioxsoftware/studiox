using System.Web;
using System.Web.Mvc;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.Web.Mvc.Configuration;
using StudioX.Web.Mvc.Extensions;

namespace StudioX.Web.Mvc.Uow
{
    public class StudioXMvcUowFilter: IActionFilter, ITransientDependency
    {
        public const string UowHttpContextKey = "__StudioXUnitOfWork";

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IStudioXMvcConfiguration _mvcConfiguration;
        private readonly IUnitOfWorkDefaultOptions _unitOfWorkDefaultOptions;

        public StudioXMvcUowFilter(
            IUnitOfWorkManager unitOfWorkManager,
            IStudioXMvcConfiguration mvcConfiguration, 
            IUnitOfWorkDefaultOptions unitOfWorkDefaultOptions)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _mvcConfiguration = mvcConfiguration;
            _unitOfWorkDefaultOptions = unitOfWorkDefaultOptions;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                return;
            }

            var methodInfo = filterContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return;
            }

            var unitOfWorkAttr =
                _unitOfWorkDefaultOptions.GetUnitOfWorkAttributeOrNull(methodInfo) ??
                _mvcConfiguration.DefaultUnitOfWorkAttribute;

            if (unitOfWorkAttr.IsDisabled)
            {
                return;
            }

            SetCurrentUow(
                filterContext.HttpContext,
                _unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions())
            );
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                return;
            }

            var uow = GetCurrentUow(filterContext.HttpContext);
            if (uow == null)
            {
                return;
            }

            try
            {
                if (filterContext.Exception == null)
                {
                    uow.Complete();
                }
            }
            finally
            {
                uow.Dispose();
                SetCurrentUow(filterContext.HttpContext, null);
            }
        }

        private static IUnitOfWorkCompleteHandle GetCurrentUow(HttpContextBase httpContext)
        {
            return httpContext.Items[UowHttpContextKey] as IUnitOfWorkCompleteHandle;
        }

        private static void SetCurrentUow(HttpContextBase httpContext, IUnitOfWorkCompleteHandle uow)
        {
            httpContext.Items[UowHttpContextKey] = uow;
        }
    }
}
