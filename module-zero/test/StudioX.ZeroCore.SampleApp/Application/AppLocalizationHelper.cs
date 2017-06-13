using StudioX.Localization;

namespace StudioX.ZeroCore.SampleApp.Application
{
    public static class AppLocalizationHelper
    {
        public static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AppConsts.LocalizationSourceName);
        }
    }
}
