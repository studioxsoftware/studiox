using StudioX.Web.Security.AntiForgery;

namespace StudioX.Web.Configuration
{
    public class StudioXWebModuleConfiguration : IStudioXWebModuleConfiguration
    {
        public IStudioXAntiForgeryWebConfiguration AntiForgery { get; }
        public IStudioXWebLocalizationConfiguration Localization { get; }

        public StudioXWebModuleConfiguration(
            IStudioXAntiForgeryWebConfiguration antiForgery, 
            IStudioXWebLocalizationConfiguration localization)
        {
            AntiForgery = antiForgery;
            Localization = localization;
        }
    }
}