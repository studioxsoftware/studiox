using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StudioX.Collections.Extensions;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.PlugIns;
using Castle.Core.Logging;

namespace StudioX.Modules
{
    /// <summary>
    /// This class is used to manage modules.
    /// </summary>
    public class StudioXModuleManager : IStudioXModuleManager
    {
        public StudioXModuleInfo StartupModule { get; private set; }

        public IReadOnlyList<StudioXModuleInfo> Modules => modules.ToImmutableList();

        public ILogger Logger { get; set; }

        private StudioXModuleCollection modules;

        private readonly IIocManager iocManager;
        private readonly IStudioXPlugInManager studioXPlugInManager;

        public StudioXModuleManager(IIocManager iocManager, IStudioXPlugInManager plugInManager)
        {
            this.iocManager = iocManager;
            studioXPlugInManager = plugInManager;

            Logger = NullLogger.Instance;
        }

        public virtual void Initialize(Type startupModule)
        {
            modules = new StudioXModuleCollection(startupModule);
            LoadAllModules();
        }

        public virtual void StartModules()
        {
            var sortedModules = modules.GetSortedModuleListByDependency();
            sortedModules.ForEach(module => module.Instance.PreInitialize());
            sortedModules.ForEach(module => module.Instance.Initialize());
            sortedModules.ForEach(module => module.Instance.PostInitialize());
        }

        public virtual void ShutdownModules()
        {
            Logger.Debug("Shutting down has been started");

            var sortedModules = modules.GetSortedModuleListByDependency();
            sortedModules.Reverse();
            sortedModules.ForEach(sm => sm.Instance.Shutdown());

            Logger.Debug("Shutting down completed.");
        }

        private void LoadAllModules()
        {
            Logger.Debug("Loading StudioX modules...");

            List<Type> plugInModuleTypes;
            var moduleTypes = FindAllModuleTypes(out plugInModuleTypes).Distinct().ToList();

            Logger.Debug("Found " + moduleTypes.Count + " StudioX modules in total.");

            RegisterModules(moduleTypes);
            CreateModules(moduleTypes, plugInModuleTypes);

            modules.EnsureKernelModuleToBeFirst();
            modules.EnsureStartupModuleToBeLast();

            SetDependencies();

            Logger.DebugFormat("{0} modules loaded.", modules.Count);
        }

        private List<Type> FindAllModuleTypes(out List<Type> plugInModuleTypes)
        {
            plugInModuleTypes = new List<Type>();

            var modules = StudioXModule.FindDependedModuleTypesRecursivelyIncludingGivenModule(this.modules.StartupModuleType);
            
            foreach (var plugInModuleType in studioXPlugInManager.PlugInSources.GetAllModules())
            {
                if (modules.AddIfNotContains(plugInModuleType))
                {
                    plugInModuleTypes.Add(plugInModuleType);
                }
            }

            return modules;
        }

        private void CreateModules(ICollection<Type> moduleTypes, List<Type> plugInModuleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                var moduleObject = iocManager.Resolve(moduleType) as StudioXModule;
                if (moduleObject == null)
                {
                    throw new StudioXInitializationException("This type is not an StudioX module: " + moduleType.AssemblyQualifiedName);
                }

                moduleObject.IocManager = iocManager;
                moduleObject.Configuration = iocManager.Resolve<IStudioXStartupConfiguration>();

                var moduleInfo = new StudioXModuleInfo(moduleType, moduleObject, plugInModuleTypes.Contains(moduleType));

                modules.Add(moduleInfo);

                if (moduleType == modules.StartupModuleType)
                {
                    StartupModule = moduleInfo;
                }

                Logger.DebugFormat("Loaded module: " + moduleType.AssemblyQualifiedName);
            }
        }

        private void RegisterModules(ICollection<Type> moduleTypes)
        {
            foreach (var moduleType in moduleTypes)
            {
                iocManager.RegisterIfNot(moduleType);
            }
        }

        private void SetDependencies()
        {
            foreach (var moduleInfo in modules)
            {
                moduleInfo.Dependencies.Clear();

                //Set dependencies for defined DependsOnAttribute attribute(s).
                foreach (var dependedModuleType in StudioXModule.FindDependedModuleTypes(moduleInfo.Type))
                {
                    var dependedModuleInfo = modules.FirstOrDefault(m => m.Type == dependedModuleType);
                    if (dependedModuleInfo == null)
                    {
                        throw new StudioXInitializationException("Could not find a depended module " + dependedModuleType.AssemblyQualifiedName + " for " + moduleInfo.Type.AssemblyQualifiedName);
                    }

                    if ((moduleInfo.Dependencies.FirstOrDefault(dm => dm.Type == dependedModuleType) == null))
                    {
                        moduleInfo.Dependencies.Add(dependedModuleInfo);
                    }
                }
            }
        }
    }
}
