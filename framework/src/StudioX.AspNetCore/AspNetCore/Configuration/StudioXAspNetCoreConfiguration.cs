using System;
using System.Collections.Generic;
using System.Reflection;
using StudioX.Domain.Uow;
using StudioX.Web.Models;

namespace StudioX.AspNetCore.Configuration
{
    public class StudioXAspNetCoreConfiguration : IStudioXAspNetCoreConfiguration
    {
        public WrapResultAttribute DefaultWrapResultAttribute { get; }

        public CacheResultAttribute DefaultCacheResultAttribute { get; }

        public UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        public List<Type> FormBodyBindingIgnoredTypes { get; }

        public ControllerAssemblySettingList ControllerAssemblySettings { get; }

        public bool IsValidationEnabledForControllers { get; set; }

        public bool IsAuditingEnabled { get; set; }

        public bool SetNoCacheForAjaxResponses { get; set; }

        public StudioXAspNetCoreConfiguration()
        {
            DefaultWrapResultAttribute = new WrapResultAttribute();
            DefaultCacheResultAttribute = new CacheResultAttribute();
            DefaultUnitOfWorkAttribute = new UnitOfWorkAttribute();
            ControllerAssemblySettings = new ControllerAssemblySettingList();
            FormBodyBindingIgnoredTypes = new List<Type>();
            IsValidationEnabledForControllers = true;
            SetNoCacheForAjaxResponses = true;
            IsAuditingEnabled = true;
        }

        public StudioXControllerAssemblySettingBuilder CreateControllersForAppServices(
            Assembly assembly,
            string moduleName = StudioXControllerAssemblySetting.DefaultServiceModuleName,
            bool useConventionalHttpVerbs = true)
        {
            var setting = new StudioXControllerAssemblySetting(moduleName, assembly, useConventionalHttpVerbs);
            ControllerAssemblySettings.Add(setting);
            return new StudioXControllerAssemblySettingBuilder(setting);
        }
    }
}