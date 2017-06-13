namespace StudioX.Web.Configuration
{
    public class StudioXWebLocalizationConfiguration : IStudioXWebLocalizationConfiguration
    {
        public string CookieName { get; set; }

        public StudioXWebLocalizationConfiguration()
        {
            CookieName = "StudioX.Localization.CultureName";
        }
    }
}