using StudioX.Web.Api.ProxyScripting.Configuration;
using StudioX.Web.MultiTenancy;
using StudioX.Web.Security.AntiForgery;

namespace StudioX.Web.Configuration
{
    /// <summary>
    /// Used to configure StudioX Web Common module.
    /// </summary>
    public interface IStudioXWebCommonModuleConfiguration
    {
        /// <summary>
        /// If this is set to true, all exception and details are sent directly to clients on an error.
        /// Default: false (StudioX hides exception details from clients except special exceptions.)
        /// </summary>
        bool SendAllExceptionsToClients { get; set; }

        /// <summary>
        /// Used to configure Api proxy scripting.
        /// </summary>
        IApiProxyScriptingConfiguration ApiProxyScripting { get; }

        /// <summary>
        /// Used to configure Anti Forgery security settings.
        /// </summary>
        IStudioXAntiForgeryConfiguration AntiForgery { get; }

        /// <summary>
        /// Used to configure embedded resource system for web applications.
        /// </summary>
        IWebEmbeddedResourcesConfiguration EmbeddedResources { get; }

        IWebMultiTenancyConfiguration MultiTenancy { get; }
    }
}