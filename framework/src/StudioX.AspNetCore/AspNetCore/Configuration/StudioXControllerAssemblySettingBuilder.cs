using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace StudioX.AspNetCore.Configuration
{
    public class StudioXControllerAssemblySettingBuilder : IStudioXControllerAssemblySettingBuilder
    {
        private readonly StudioXControllerAssemblySetting _setting;

        public StudioXControllerAssemblySettingBuilder(StudioXControllerAssemblySetting setting)
        {
            _setting = setting;
        }

        public StudioXControllerAssemblySettingBuilder Where(Func<Type, bool> predicate)
        {
            _setting.TypePredicate = predicate;
            return this;
        }

        public StudioXControllerAssemblySettingBuilder ConfigureControllerModel(Action<ControllerModel> configurer)
        {
            _setting.ControllerModelConfigurer = configurer;
            return this;
        }
    }
}