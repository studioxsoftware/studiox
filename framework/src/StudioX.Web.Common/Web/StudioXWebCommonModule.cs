using System.Reflection;
using StudioX.Configuration.Startup;
using StudioX.Localization.Dictionaries;
using StudioX.Localization.Dictionaries.Xml;
using StudioX.Modules;
using StudioX.Web.Api.ProxyScripting.Configuration;
using StudioX.Web.Api.ProxyScripting.Generators.JQuery;
using StudioX.Web.Configuration;
using StudioX.Web.MultiTenancy;
using StudioX.Web.Security.AntiForgery;
using StudioX.Reflection.Extensions;

namespace StudioX.Web
{
    /// <summary>
    /// This module is used to use StudioX in ASP.NET web applications.
    /// </summary>
    [DependsOn(typeof(StudioXKernelModule))]    
    public class StudioXWebCommonModule : StudioXModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.Register<IWebMultiTenancyConfiguration, WebMultiTenancyConfiguration>();
            IocManager.Register<IApiProxyScriptingConfiguration, ApiProxyScriptingConfiguration>();
            IocManager.Register<IStudioXAntiForgeryConfiguration, StudioXAntiForgeryConfiguration>();
            IocManager.Register<IWebEmbeddedResourcesConfiguration, WebEmbeddedResourcesConfiguration>();
            IocManager.Register<IStudioXWebCommonModuleConfiguration, StudioXWebCommonModuleConfiguration>();

            Configuration.Modules.StudioXWebCommon().ApiProxyScripting.Generators[JQueryProxyScriptGenerator.Name] = typeof(JQueryProxyScriptGenerator);

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    StudioXWebConsts.LocalizaionSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(StudioXWebCommonModule).GetAssembly(), "StudioX.Web.Localization.StudioXWebXmlSource"
                        )));
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXWebCommonModule).GetAssembly());            
        }
    }
}
