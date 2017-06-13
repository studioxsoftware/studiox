using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace StudioX.AspNetCore.Configuration
{
    public interface IStudioXControllerAssemblySettingBuilder
    {
        StudioXControllerAssemblySettingBuilder Where(Func<Type, bool> predicate);

        StudioXControllerAssemblySettingBuilder ConfigureControllerModel(Action<ControllerModel> configurer);
    }
}