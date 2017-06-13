using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StudioX.Modules;

namespace StudioX.Reflection
{
    public class StudioXAssemblyFinder : IAssemblyFinder
    {
        private readonly IStudioXModuleManager moduleManager;

        public StudioXAssemblyFinder(IStudioXModuleManager moduleManager)
        {
            this.moduleManager = moduleManager;
        }

        public List<Assembly> GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var module in moduleManager.Modules)
            {
                assemblies.Add(module.Assembly);
                assemblies.AddRange(module.Instance.GetAdditionalAssemblies());
            }

            return assemblies.Distinct().ToList();
        }
    }
}