using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using StudioX.Application.Features;
using StudioX.Application.Navigation;
using StudioX.Application.Services;
using StudioX.Auditing;
using StudioX.Authorization;
using StudioX.BackgroundJobs;
using StudioX.Collections.Extensions;
using StudioX.Configuration;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.Events.Bus;
using StudioX.Localization;
using StudioX.Localization.Dictionaries;
using StudioX.Localization.Dictionaries.Xml;
using StudioX.Modules;
using StudioX.MultiTenancy;
using StudioX.Net.Mail;
using StudioX.Notifications;
using StudioX.Reflection.Extensions;
using StudioX.Runtime;
using StudioX.Runtime.Caching;
using StudioX.Runtime.Remoting;
using StudioX.Runtime.Validation.Interception;
using StudioX.Threading;
using StudioX.Threading.BackgroundWorkers;
using StudioX.Timing;
using Castle.MicroKernel.Registration;

namespace StudioX
{
    /// <summary>
    /// Kernel (core) module of the StudioX system.
    /// No need to depend on this, it's automatically the first module always.
    /// </summary>
    public sealed class StudioXKernelModule : StudioXModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            IocManager.Register<IScopedIocResolver, ScopedIocResolver>(DependencyLifeStyle.Transient);
            IocManager.Register(typeof(IAmbientScopeProvider<>), typeof(DataContextAmbientScopeProvider<>), DependencyLifeStyle.Transient);

            AddAuditingSelectors();
            AddLocalizationSources();
            AddSettingProviders();
            AddUnitOfWorkFilters();
            ConfigureCaches();
            AddIgnoredTypes();
        }

        public override void Initialize()
        {
            foreach (var replaceAction in ((StudioXStartupConfiguration)Configuration).ServiceReplaceActions.Values)
            {
                replaceAction();
            }

            IocManager.IocContainer.Install(new EventBusInstaller(IocManager));

            IocManager.RegisterAssemblyByConvention(typeof(StudioXKernelModule).GetAssembly(),
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });
        }

        public override void PostInitialize()
        {
            RegisterMissingComponents();

            IocManager.Resolve<SettingDefinitionManager>().Initialize();
            IocManager.Resolve<FeatureManager>().Initialize();
            IocManager.Resolve<PermissionManager>().Initialize();
            IocManager.Resolve<LocalizationManager>().Initialize();
            IocManager.Resolve<NotificationDefinitionManager>().Initialize();
            IocManager.Resolve<NavigationManager>().Initialize();

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                var workerManager = IocManager.Resolve<IBackgroundWorkerManager>();
                workerManager.Start();
                workerManager.Add(IocManager.Resolve<IBackgroundJobManager>());
            }
        }

        public override void Shutdown()
        {
            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                IocManager.Resolve<IBackgroundWorkerManager>().StopAndWaitToStop();
            }
        }

        private void AddUnitOfWorkFilters()
        {
            Configuration.UnitOfWork.RegisterFilter(StudioXDataFilters.SoftDelete, true);
            Configuration.UnitOfWork.RegisterFilter(StudioXDataFilters.MustHaveTenant, true);
            Configuration.UnitOfWork.RegisterFilter(StudioXDataFilters.MayHaveTenant, true);
        }

        private void AddSettingProviders()
        {
            Configuration.Settings.Providers.Add<LocalizationSettingProvider>();
            Configuration.Settings.Providers.Add<EmailSettingProvider>();
            Configuration.Settings.Providers.Add<NotificationSettingProvider>();
            Configuration.Settings.Providers.Add<TimingSettingProvider>();
        }

        private void AddAuditingSelectors()
        {
            Configuration.Auditing.Selectors.Add(
                new NamedTypeSelector(
                    "StudioX.ApplicationServices",
                    type => typeof(IApplicationService).IsAssignableFrom(type)
                )
            );
        }

        private void AddLocalizationSources()
        {
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    StudioXConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(StudioXKernelModule).GetAssembly(), "StudioX.Localization.Sources.StudioXXmlSource"
                    )));
        }

        private void ConfigureCaches()
        {
            Configuration.Caching.Configure(StudioXCacheNames.ApplicationSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromHours(8);
            });

            Configuration.Caching.Configure(StudioXCacheNames.TenantSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(60);
            });

            Configuration.Caching.Configure(StudioXCacheNames.UserSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(20);
            });
        }

        private void AddIgnoredTypes()
        {
            var commonIgnoredTypes = new[]
            {
                typeof(Stream),
                typeof(Expression)
            };

            foreach (var ignoredType in commonIgnoredTypes)
            {
                Configuration.Auditing.IgnoredTypes.AddIfNotContains(ignoredType);
                Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }

            var validationIgnoredTypes = new[] { typeof(Type) };
            foreach (var ignoredType in validationIgnoredTypes)
            {
                Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }
        }

        private void RegisterMissingComponents()
        {
            if (!IocManager.IsRegistered<IGuidGenerator>())
            {
                IocManager.IocContainer.Register(
                    Component
                        .For<IGuidGenerator, SequentialGuidGenerator>()
                        .Instance(SequentialGuidGenerator.Instance)
                );
            }

            IocManager.RegisterIfNot<IUnitOfWork, NullUnitOfWork>(DependencyLifeStyle.Transient);
            IocManager.RegisterIfNot<IAuditingStore, SimpleLogAuditingStore>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<IPermissionChecker, NullPermissionChecker>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<IRealTimeNotifier, NullRealTimeNotifier>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<INotificationStore, NullNotificationStore>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<IUnitOfWorkFilterExecuter, NullUnitOfWorkFilterExecuter>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<IClientInfoProvider, NullClientInfoProvider>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<ITenantStore, NullTenantStore>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<ITenantResolverCache, NullTenantResolverCache>(DependencyLifeStyle.Singleton);

            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                IocManager.RegisterIfNot<IBackgroundJobStore, InMemoryBackgroundJobStore>(DependencyLifeStyle.Singleton);
            }
            else
            {
                IocManager.RegisterIfNot<IBackgroundJobStore, NullBackgroundJobStore>(DependencyLifeStyle.Singleton);
            }
        }
    }
}