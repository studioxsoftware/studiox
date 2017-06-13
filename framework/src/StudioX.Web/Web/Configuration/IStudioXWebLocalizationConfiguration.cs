namespace StudioX.Web.Configuration
{
    public interface IStudioXWebLocalizationConfiguration
    {
        /// <summary>
        /// Default: "StudioX.Localization.CultureName".
        /// </summary>
        string CookieName { get; set; }
    }
}