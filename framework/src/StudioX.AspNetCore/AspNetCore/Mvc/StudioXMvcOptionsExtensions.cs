using StudioX.AspNetCore.Mvc.Auditing;
using StudioX.AspNetCore.Mvc.Authorization;
using StudioX.AspNetCore.Mvc.Conventions;
using StudioX.AspNetCore.Mvc.ExceptionHandling;
using StudioX.AspNetCore.Mvc.ModelBinding;
using StudioX.AspNetCore.Mvc.Results;
using StudioX.AspNetCore.Mvc.Uow;
using StudioX.AspNetCore.Mvc.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace StudioX.AspNetCore.Mvc
{
    internal static class StudioXMvcOptionsExtensions
    {
        public static void AddStudioX(this MvcOptions options, IServiceCollection services)
        {
            AddConventions(options, services);
            AddFilters(options);
            AddModelBinders(options);
        }

        private static void AddConventions(MvcOptions options, IServiceCollection services)
        {
            options.Conventions.Add(new StudioXAppServiceConvention(services));
        }

        private static void AddFilters(MvcOptions options)
        {
            options.Filters.AddService(typeof(StudioXAuthorizationFilter));
            options.Filters.AddService(typeof(StudioXAuditActionFilter));
            options.Filters.AddService(typeof(StudioXValidationActionFilter));
            options.Filters.AddService(typeof(StudioXUowActionFilter));
            options.Filters.AddService(typeof(StudioXExceptionFilter));
            options.Filters.AddService(typeof(StudioXResultFilter));
        }

        private static void AddModelBinders(MvcOptions options)
        {
            options.ModelBinderProviders.Add(new StudioXDateTimeModelBinderProvider());
        }
    }
}