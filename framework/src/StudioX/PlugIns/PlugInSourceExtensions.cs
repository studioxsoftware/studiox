using System;
using System.Collections.Generic;
using System.Linq;
using StudioX.Modules;

namespace StudioX.PlugIns
{
    public static class PlugInSourceExtensions
    {
        public static List<Type> GetModulesWithAllDependencies(this IPlugInSource plugInSource)
        {
            return plugInSource
                .GetModules()
                .SelectMany(StudioXModule.FindDependedModuleTypesRecursivelyIncludingGivenModule)
                .Distinct()
                .ToList();
        }
    }
}