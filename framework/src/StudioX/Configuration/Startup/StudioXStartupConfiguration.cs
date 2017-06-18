using System;
using System.Collections.Generic;
using StudioX.Application.Features;
using StudioX.Auditing;
using StudioX.BackgroundJobs;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.Events.Bus;
using StudioX.Notifications;
using StudioX.Resources.Embedded;
using StudioX.Runtime.Caching.Configuration;

namespace StudioX.Configuration.Startup
{
    /// <summary>
    ///     This class is used to configure StudioX and modules on startup.
    /// </summary>
    internal class StudioXStartupConfiguration : DictionaryBasedConfig, IStudioXStartupConfiguration
    {
        /// <summary>
        ///     Reference to the IocManager.
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        ///     Used to set localization configuration.
        /// </summary>
        public ILocalizationConfiguration Localization { get; private set; }

        /// <summary>
        ///     Used to configure authorization.
        /// </summary>
        public IAuthorizationConfiguration Authorization { get; private set; }

        /// <summary>
        ///     Used to configure validation.
        /// </summary>
        public IValidationConfiguration Validation { get; private set; }

        /// <summary>
        ///     Used to configure settings.
        /// </summary>
        public ISettingsConfiguration Settings { get; private set; }

        /// <summary>
        ///     Gets/sets default connection string used by ORM module.
        ///     It can be name of a connection string in application's config file or can be full connection string.
        /// </summary>
        public string DefaultNameOrConnectionString { get; set; }

        /// <summary>
        ///     Used to configure modules.
        ///     Modules can write extension methods to <see cref="ModuleConfigurations" /> to add module specific configurations.
        /// </summary>
        public IModuleConfigurations Modules { get; private set; }

        /// <summary>
        ///     Used to configure unit of work defaults.
        /// </summary>
        public IUnitOfWorkDefaultOptions UnitOfWork { get; private set; }

        /// <summary>
        ///     Used to configure features.
        /// </summary>
        public IFeatureConfiguration Features { get; private set; }

        /// <summary>
        ///     Used to configure background job system.
        /// </summary>
        public IBackgroundJobConfiguration BackgroundJobs { get; private set; }

        /// <summary>
        ///     Used to configure notification system.
        /// </summary>
        public INotificationConfiguration Notifications { get; private set; }

        /// <summary>
        ///     Used to configure navigation.
        /// </summary>
        public INavigationConfiguration Navigation { get; private set; }

        /// <summary>
        ///     Used to configure <see cref="IEventBus" />.
        /// </summary>
        public IEventBusConfiguration EventBus { get; private set; }

        /// <summary>
        ///     Used to configure auditing.
        /// </summary>
        public IAuditingConfiguration Auditing { get; private set; }

        public ICachingConfiguration Caching { get; private set; }

        /// <summary>
        ///     Used to configure multi-tenancy.
        /// </summary>
        public IMultiTenancyConfig MultiTenancy { get; private set; }

        public Dictionary<Type, Action> ServiceReplaceActions { get; private set; }

        public IEmbeddedResourcesConfiguration EmbeddedResources { get; private set; }

        /// <summary>
        ///     Private constructor for singleton pattern.
        /// </summary>
        public StudioXStartupConfiguration(IIocManager iocManager)
        {
            IocManager = iocManager;
        }

        public void Initialize()
        {
            Localization = IocManager.Resolve<ILocalizationConfiguration>();
            Modules = IocManager.Resolve<IModuleConfigurations>();
            Features = IocManager.Resolve<IFeatureConfiguration>();
            Navigation = IocManager.Resolve<INavigationConfiguration>();
            Authorization = IocManager.Resolve<IAuthorizationConfiguration>();
            Validation = IocManager.Resolve<IValidationConfiguration>();
            Settings = IocManager.Resolve<ISettingsConfiguration>();
            UnitOfWork = IocManager.Resolve<IUnitOfWorkDefaultOptions>();
            EventBus = IocManager.Resolve<IEventBusConfiguration>();
            MultiTenancy = IocManager.Resolve<IMultiTenancyConfig>();
            Auditing = IocManager.Resolve<IAuditingConfiguration>();
            Caching = IocManager.Resolve<ICachingConfiguration>();
            BackgroundJobs = IocManager.Resolve<IBackgroundJobConfiguration>();
            Notifications = IocManager.Resolve<INotificationConfiguration>();
            EmbeddedResources = IocManager.Resolve<IEmbeddedResourcesConfiguration>();

            ServiceReplaceActions = new Dictionary<Type, Action>();
        }

        public void ReplaceService(Type type, Action replaceAction)
        {
            ServiceReplaceActions[type] = replaceAction;
        }

        public T Get<T>()
        {
            return GetOrCreate(typeof(T).FullName, () => IocManager.Resolve<T>());
        }
    }
}