using System;
using System.Collections.Generic;
using System.Linq;
using StudioX.Collections.Extensions;

namespace StudioX.Modules
{
    /// <summary>
    ///     Used to store StudioXModuleInfo objects as a dictionary.
    /// </summary>
    internal class StudioXModuleCollection : List<StudioXModuleInfo>
    {
        public Type StartupModuleType { get; }

        public StudioXModuleCollection(Type startupModuleType)
        {
            StartupModuleType = startupModuleType;
        }

        /// <summary>
        ///     Gets a reference to a module instance.
        /// </summary>
        /// <typeparam name="TModule">Module type</typeparam>
        /// <returns>Reference to the module instance</returns>
        public TModule GetModule<TModule>() where TModule : StudioXModule
        {
            var module = this.FirstOrDefault(m => m.Type == typeof(TModule));
            if (module == null)
            {
                throw new StudioXException("Can not find module for " + typeof(TModule).FullName);
            }

            return (TModule) module.Instance;
        }

        /// <summary>
        ///     Sorts modules according to dependencies.
        ///     If module A depends on module B, A comes after B in the returned List.
        /// </summary>
        /// <returns>Sorted list</returns>
        public List<StudioXModuleInfo> GetSortedModuleListByDependency()
        {
            var sortedModules = this.SortByDependencies(x => x.Dependencies);
            EnsureKernelModuleToBeFirst(sortedModules);
            EnsureStartupModuleToBeLast(sortedModules, StartupModuleType);
            return sortedModules;
        }

        public static void EnsureKernelModuleToBeFirst(List<StudioXModuleInfo> modules)
        {
            var kernelModuleIndex = modules.FindIndex(m => m.Type == typeof(StudioXKernelModule));
            if (kernelModuleIndex <= 0)
            {
                //It's already the first!
                return;
            }

            var kernelModule = modules[kernelModuleIndex];
            modules.RemoveAt(kernelModuleIndex);
            modules.Insert(0, kernelModule);
        }

        public static void EnsureStartupModuleToBeLast(List<StudioXModuleInfo> modules, Type startupModuleType)
        {
            var startupModuleIndex = modules.FindIndex(m => m.Type == startupModuleType);
            if (startupModuleIndex >= modules.Count - 1)
            {
                //It's already the last!
                return;
            }

            var startupModule = modules[startupModuleIndex];
            modules.RemoveAt(startupModuleIndex);
            modules.Add(startupModule);
        }

        public void EnsureKernelModuleToBeFirst()
        {
            EnsureKernelModuleToBeFirst(this);
        }

        public void EnsureStartupModuleToBeLast()
        {
            EnsureStartupModuleToBeLast(this, StartupModuleType);
        }
    }
}