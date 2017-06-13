using StudioX.Collections;

namespace StudioX.Configuration.Startup
{
    internal class SettingsConfiguration : ISettingsConfiguration
    {
        public ITypeList<SettingProvider> Providers { get; private set; }

        public SettingsConfiguration()
        {
            Providers = new TypeList<SettingProvider>();
        }
    }
}