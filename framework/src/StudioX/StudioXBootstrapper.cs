using System;
using System.Reflection;
using StudioX.Auditing;
using StudioX.Authorization;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Dependency.Installers;
using StudioX.Domain.Uow;
using StudioX.Modules;
using StudioX.PlugIns;
using StudioX.Runtime.Validation.Interception;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using JetBrains.Annotations;

namespace StudioX
{
    /// <summary>
    /// This is the main class that is responsible to start entire StudioX system.
    /// Prepares dependency injection and registers core components needed for startup.
    /// It must be instantiated and initialized (see <see cref="Initialize"/>) first in an application.
    /// </summary>
    public class StudioXBootstrapper : IDisposable
    {
        /// <summary>
        /// Get the startup module of the application which depends on other used modules.
        /// </summary>
        public Type StartupModule { get; }

        /// <summary>
        /// A list of plug in folders.
        /// </summary>
        public PlugInSourceList PlugInSources { get; }

        /// <summary>
        /// Gets IIocManager object used by this class.
        /// </summary>
        public IIocManager IocManager { get; }

        /// <summary>
        /// Is this object disposed before?
        /// </summary>
        protected bool IsDisposed;

        private StudioXModuleManager _moduleManager;
        private ILogger _logger;

        /// <summary>
        /// Creates a new <see cref="StudioXBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="StudioXModule"/>.</param>
        private StudioXBootstrapper([NotNull] Type startupModule)
            : this(startupModule, Dependency.IocManager.Instance)
        {

        }

        /// <summary>
        /// Creates a new <see cref="StudioXBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="StudioXModule"/>.</param>
        /// <param name="iocManager">IIocManager that is used to bootstrap the StudioX system</param>
        private StudioXBootstrapper([NotNull] Type startupModule, [NotNull] IIocManager iocManager)
        {
            Check.NotNull(startupModule, nameof(startupModule));
            Check.NotNull(iocManager, nameof(iocManager));

            if (!typeof(StudioXModule).GetTypeInfo().IsAssignableFrom(startupModule))
            {
                throw new ArgumentException($"{nameof(startupModule)} should be derived from {nameof(StudioXModule)}.");
            }

            StartupModule = startupModule;
            IocManager = iocManager;

            PlugInSources = new PlugInSourceList();
            _logger = NullLogger.Instance;

            AddInterceptorRegistrars();
        }

        /// <summary>
        /// Creates a new <see cref="StudioXBootstrapper"/> instance.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="StudioXModule"/>.</typeparam>
        public static StudioXBootstrapper Create<TStartupModule>()
            where TStartupModule : StudioXModule
        {
            return new StudioXBootstrapper(typeof(TStartupModule));
        }

        /// <summary>
        /// Creates a new <see cref="StudioXBootstrapper"/> instance.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="StudioXModule"/>.</typeparam>
        /// <param name="iocManager">IIocManager that is used to bootstrap the StudioX system</param>
        public static StudioXBootstrapper Create<TStartupModule>([NotNull] IIocManager iocManager)
            where TStartupModule : StudioXModule
        {
            return new StudioXBootstrapper(typeof(TStartupModule), iocManager);
        }

        /// <summary>
        /// Creates a new <see cref="StudioXBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="StudioXModule"/>.</param>
        public static StudioXBootstrapper Create([NotNull] Type startupModule)
        {
            return new StudioXBootstrapper(startupModule);
        }

        /// <summary>
        /// Creates a new <see cref="StudioXBootstrapper"/> instance.
        /// </summary>
        /// <param name="startupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="StudioXModule"/>.</param>
        /// <param name="iocManager">IIocManager that is used to bootstrap the StudioX system</param>
        public static StudioXBootstrapper Create([NotNull] Type startupModule, [NotNull] IIocManager iocManager)
        {
            return new StudioXBootstrapper(startupModule, iocManager);
        }

        private void AddInterceptorRegistrars()
        {
            ValidationInterceptorRegistrar.Initialize(IocManager);
            AuditingInterceptorRegistrar.Initialize(IocManager);
            UnitOfWorkRegistrar.Initialize(IocManager);
            AuthorizationInterceptorRegistrar.Initialize(IocManager);
        }

        /// <summary>
        /// Initializes the StudioX system.
        /// </summary>
        public virtual void Initialize()
        {
            ResolveLogger();

            try
            {
                RegisterBootstrapper();
                IocManager.IocContainer.Install(new StudioXCoreInstaller());

                IocManager.Resolve<StudioXPlugInManager>().PlugInSources.AddRange(PlugInSources);
                IocManager.Resolve<StudioXStartupConfiguration>().Initialize();

                _moduleManager = IocManager.Resolve<StudioXModuleManager>();
                _moduleManager.Initialize(StartupModule);
                _moduleManager.StartModules();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.ToString(), ex);
                throw;
            }
        }

        private void ResolveLogger()
        {
            if (IocManager.IsRegistered<ILoggerFactory>())
            {
                _logger = IocManager.Resolve<ILoggerFactory>().Create(typeof(StudioXBootstrapper));
            }
        }

        private void RegisterBootstrapper()
        {
            if (!IocManager.IsRegistered<StudioXBootstrapper>())
            {
                IocManager.IocContainer.Register(
                    Component.For<StudioXBootstrapper>().Instance(this)
                    );
            }
        }

        /// <summary>
        /// Disposes the StudioX system.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            _moduleManager?.ShutdownModules();
        }
    }
}
