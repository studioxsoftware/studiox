using System;
using System.Collections.Generic;
using System.Reflection;
using StudioX.Domain.Uow;
using StudioX.Web.Models;

namespace StudioX.AspNetCore.Configuration
{
    public interface IStudioXAspNetCoreConfiguration
    {
        WrapResultAttribute DefaultWrapResultAttribute { get; }

        CacheResultAttribute DefaultCacheResultAttribute { get; }

        UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        List<Type> FormBodyBindingIgnoredTypes { get; }

        /// <summary>
        /// Default: true.
        /// </summary>
        bool IsValidationEnabledForControllers { get; set; }

        /// <summary>
        /// Used to enable/disable auditing for MVC controllers.
        /// Default: true.
        /// </summary>
        bool IsAuditingEnabled { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        bool SetNoCacheForAjaxResponses { get; set; }

        StudioXControllerAssemblySettingBuilder CreateControllersForAppServices(
            Assembly assembly,
            string moduleName = StudioXControllerAssemblySetting.DefaultServiceModuleName,
            bool useConventionalHttpVerbs = true
        );
    }
}
