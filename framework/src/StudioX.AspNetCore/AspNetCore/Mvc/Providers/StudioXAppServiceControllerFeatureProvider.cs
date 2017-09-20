using System.Linq;
using System.Reflection;
using StudioX.Application.Services;
using StudioX.AspNetCore.Configuration;
using StudioX.Dependency;
using StudioX.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace StudioX.AspNetCore.Mvc.Providers
{
    /// <summary>
    /// Used to add application services as controller.
    /// </summary>
    public class StudioXAppServiceControllerFeatureProvider : ControllerFeatureProvider
    {
        private readonly IIocResolver _iocResolver;

        public StudioXAppServiceControllerFeatureProvider(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        protected override bool IsController(TypeInfo typeInfo)
        {
            var type = typeInfo.AsType();

            if (!typeof(IApplicationService).IsAssignableFrom(type) ||
                !typeInfo.IsPublic || typeInfo.IsAbstract || typeInfo.IsGenericType)
            {
                return false;
            }

            var remoteServiceAttr = ReflectionHelper.GetSingleAttributeOrDefault<RemoteServiceAttribute>(typeInfo);

            if (remoteServiceAttr != null && !remoteServiceAttr.IsEnabledFor(type))
            {
                return false;
            }

            var configuration = _iocResolver.Resolve<StudioXAspNetCoreConfiguration>().ControllerAssemblySettings.GetSettingOrNull(type);
            return configuration != null && configuration.TypePredicate(type);
        }
    }
}