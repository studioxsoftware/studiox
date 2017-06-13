using System;
using System.Collections.Generic;
using System.Linq;
using StudioX.Reflection.Extensions;
using JetBrains.Annotations;

namespace StudioX.AspNetCore.Configuration
{
    public class ControllerAssemblySettingList : List<StudioXControllerAssemblySetting>
    {
        [CanBeNull]
        public StudioXControllerAssemblySetting GetSettingOrNull(Type controllerType)
        {
            return this.FirstOrDefault(controllerSetting => controllerSetting.Assembly == controllerType.GetAssembly());
        }
    }
}