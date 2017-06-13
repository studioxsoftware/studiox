using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using StudioX.Collections.Extensions;
using StudioX.Modules;
using StudioX.Reflection;

namespace StudioX.PlugIns
{
    public class FolderPlugInSource : IPlugInSource
    {
        public string Folder { get; }

        public SearchOption SearchOption { get; set; }

        private readonly Lazy<List<Assembly>> assemblies;
        
        public FolderPlugInSource(string folder, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Folder = folder;
            SearchOption = searchOption;

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
            return AssemblyHelper.GetAllAssembliesInFolder(Folder, SearchOption);
        }
    }
}