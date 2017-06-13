using System.Web.Mvc;
using StudioX.Dependency;
using StudioX.Web.Mvc.Configuration;
using StudioX.Web.Mvc.Extensions;

namespace StudioX.Web.Mvc.Validation
{
    public class StudioXMvcValidationFilter : IActionFilter, ITransientDependency
    {
        private readonly IIocResolver iocResolver;
        private readonly IStudioXMvcConfiguration configuration;

        public StudioXMvcValidationFilter(IIocResolver iocResolver, IStudioXMvcConfiguration configuration)
        {
            this.iocResolver = iocResolver;
            this.configuration = configuration;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!configuration.IsValidationEnabledForControllers)
            {
                return;
            }

            var methodInfo = filterContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return;
            }

            using (var validator = iocResolver.ResolveAsDisposable<MvcActionInvocationValidator>())
            {
                validator.Object.Initialize(filterContext, methodInfo);
                validator.Object.Validate();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }
    }
}
