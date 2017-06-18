using StudioX.Collections;

namespace StudioX.Configuration.Startup
{
    internal class SettingsConfiguration : ISettingsConfiguration
    {
        public ITypeList<SettingProvider> Providers { get; }

        public SettingsConfiguration()
        {
            Providers = new TypeList<SettingProvider>();
        }
    }
}