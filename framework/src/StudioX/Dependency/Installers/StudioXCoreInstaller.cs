using StudioX.Application.Features;
using StudioX.Auditing;
using StudioX.BackgroundJobs;
using StudioX.Configuration.Startup;
using StudioX.Domain.Uow;
using StudioX.Localization;
using StudioX.Modules;
using StudioX.Notifications;
using StudioX.PlugIns;
using StudioX.Reflection;
using StudioX.Resources.Embedded;
using StudioX.Runtime.Caching.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace StudioX.Dependency.Installers
{
    internal class StudioXCoreInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IUnitOfWorkDefaultOptions, UnitOfWorkDefaultOptions>().ImplementedBy<UnitOfWorkDefaultOptions>().LifestyleSingleton(),
                Component.For<INavigationConfiguration, NavigationConfiguration>().ImplementedBy<NavigationConfiguration>().LifestyleSingleton(),
                Component.For<ILocalizationConfiguration, LocalizationConfiguration>().ImplementedBy<LocalizationConfiguration>().LifestyleSingleton(),
                Component.For<IAuthorizationConfiguration, AuthorizationConfiguration>().ImplementedBy<AuthorizationConfiguration>().LifestyleSingleton(),
                Component.For<IValidationConfiguration, ValidationConfiguration>().ImplementedBy<ValidationConfiguration>().LifestyleSingleton(),
                Component.For<IFeatureConfiguration, FeatureConfiguration>().ImplementedBy<FeatureConfiguration>().LifestyleSingleton(),
                Component.For<ISettingsConfiguration, SettingsConfiguration>().ImplementedBy<SettingsConfiguration>().LifestyleSingleton(),
                Component.For<IModuleConfigurations, ModuleConfigurations>().ImplementedBy<ModuleConfigurations>().LifestyleSingleton(),
                Component.For<IEventBusConfiguration, EventBusConfiguration>().ImplementedBy<EventBusConfiguration>().LifestyleSingleton(),
                Component.For<IMultiTenancyConfig, MultiTenancyConfig>().ImplementedBy<MultiTenancyConfig>().LifestyleSingleton(),
                Component.For<ICachingConfiguration, CachingConfiguration>().ImplementedBy<CachingConfiguration>().LifestyleSingleton(),
                Component.For<IAuditingConfiguration, AuditingConfiguration>().ImplementedBy<AuditingConfiguration>().LifestyleSingleton(),
                Component.For<IBackgroundJobConfiguration, BackgroundJobConfiguration>().ImplementedBy<BackgroundJobConfiguration>().LifestyleSingleton(),
                Component.For<INotificationConfiguration, NotificationConfiguration>().ImplementedBy<NotificationConfiguration>().LifestyleSingleton(),
                Component.For<IEmbeddedResourcesConfiguration, EmbeddedResourcesConfiguration>().ImplementedBy<EmbeddedResourcesConfiguration>().LifestyleSingleton(),
                Component.For<IStudioXStartupConfiguration, StudioXStartupConfiguration>().ImplementedBy<StudioXStartupConfiguration>().LifestyleSingleton(),
                Component.For<ITypeFinder, TypeFinder>().ImplementedBy<TypeFinder>().LifestyleSingleton(),
                Component.For<IStudioXPlugInManager, StudioXPlugInManager>().ImplementedBy<StudioXPlugInManager>().LifestyleSingleton(),
                Component.For<IStudioXModuleManager, StudioXModuleManager>().ImplementedBy<StudioXModuleManager>().LifestyleSingleton(),
                Component.For<IAssemblyFinder, StudioXAssemblyFinder>().ImplementedBy<StudioXAssemblyFinder>().LifestyleSingleton(),
                Component.For<ILocalizationManager, LocalizationManager>().ImplementedBy<LocalizationManager>().LifestyleSingleton()
                );
        }
    }
}
