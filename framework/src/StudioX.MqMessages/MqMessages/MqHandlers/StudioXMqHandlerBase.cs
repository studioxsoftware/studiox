using System;
using System.Threading.Tasks;
using StudioX.Dependency;

namespace StudioX.MqMessages.MqHandlers
{
    /// <summary>
    ///     a Base class like DomainService
    /// </summary>
    public abstract class StudioXMqHandlerBase : StudioXServiceBase, ITransientDependency
    {
        public StudioXMqHandlerBase(string localizationSourceName)
        {
            LocalizationSourceName = localizationSourceName;
        }

        protected async Task<bool> IsTrueSetting(string settingKey)
        {
            return string.Equals("true", await SettingManager.GetSettingValueForApplicationAsync(settingKey),
                StringComparison.CurrentCultureIgnoreCase);
        }
    }
}