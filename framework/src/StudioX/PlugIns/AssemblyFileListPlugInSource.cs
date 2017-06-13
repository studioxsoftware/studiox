using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StudioX.Collections.Extensions;
using StudioX.Modules;

#if !NET46
using System.Runtime.Loader;
#endif

namespace StudioX.PlugIns
{
    //TODO: This class is similar to FolderPlugInSource. Create an abstract base class for them.
    public class AssemblyFileListPlugInSource : IPlugInSource
    {
        public string[] FilePaths { get; }

        private readonly Lazy<List<Assembly>> assemblies;

        public AssemblyFileListPlugInSource(params string[] filePaths)
        {
            FilePaths = filePaths ?? new string[0];

            assemblies = new Lazy<List<Assembly>>(LoadAssemblies, true);
        }

        public List<Assembly> GetAssemblies()
        {
            return assemblies.Value;
        }

        public List<Type> GetModules()
        {
            var modules = new List<Type>();

            foreach (var assembly in GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (StudioXModule.IsStudioXModule(type))
                        {
                            modules.AddIfNotContains(type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new StudioXInitializationException("Could not get module types from assembly: " + assembly.FullName, ex);
                }
            }

            return modules;
        }

        private List<Assembly> LoadAssemblies()
        {
            return FilePaths.Select(
#if NET46
                Assembly.LoadFile
#else
                AssemblyLoadContext.Default.LoadFromAssemblyPath
#endif
                ).ToList();
        }
    }
}