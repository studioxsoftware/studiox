using StudioX.Web.Security.AntiForgery;

namespace StudioX.Web.Configuration
{
    public interface IStudioXWebModuleConfiguration
    {
        IStudioXAntiForgeryWebConfiguration AntiForgery { get; }

        IStudioXWebLocalizationConfiguration Localization { get; }
    }
}