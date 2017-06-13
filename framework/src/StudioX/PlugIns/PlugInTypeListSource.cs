using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StudioX.PlugIns
{
    public class PlugInTypeListSource : IPlugInSource
    {
        private readonly Type[] moduleTypes;
        private readonly Lazy<List<Assembly>> assemblies;

        public PlugInTypeListSource(params Type[] moduleTypes)
        {
            this.moduleTypes = moduleTypes;
            assemblies = new Lazy<List<Assembly>>(LoadAssemblies, true);
        }

        public List<Assembly> GetAssemblies()
        {
            return assemblies.Value;
        }

        public List<Type> GetModules()
        {
            return moduleTypes.ToList();
        }

        private List<Assembly> LoadAssemblies()
        {
            return moduleTypes.Select(type => type.GetTypeInfo().Assembly).ToList();
        }
    }
}