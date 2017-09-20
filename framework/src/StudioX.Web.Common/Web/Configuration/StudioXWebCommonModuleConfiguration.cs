using StudioX.Web.Api.ProxyScripting.Configuration;
using StudioX.Web.MultiTenancy;
using StudioX.Web.Security.AntiForgery;

namespace StudioX.Web.Configuration
{
    internal class StudioXWebCommonModuleConfiguration : IStudioXWebCommonModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }

        public IApiProxyScriptingConfiguration ApiProxyScripting { get; }

        public IStudioXAntiForgeryConfiguration AntiForgery { get; }

        public IWebEmbeddedResourcesConfiguration EmbeddedResources { get; }

        public IWebMultiTenancyConfiguration MultiTenancy { get; }

        public StudioXWebCommonModuleConfiguration(
            IApiProxyScriptingConfiguration apiProxyScripting, 
            IStudioXAntiForgeryConfiguration studioxAntiForgery,
            IWebEmbeddedResourcesConfiguration embeddedResources, 
            IWebMultiTenancyConfiguration multiTenancy)
        {
            ApiProxyScripting = apiProxyScripting;
            AntiForgery = studioxAntiForgery;
            EmbeddedResources = embeddedResources;
            MultiTenancy = multiTenancy;
        }
    }
}