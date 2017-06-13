using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StudioX.Collections.Extensions;
using Castle.Core.Logging;

namespace StudioX.Reflection
{
    public class TypeFinder : ITypeFinder
    {
        public ILogger Logger { get; set; }

        private readonly IAssemblyFinder assemblyFinder;
        private readonly object syncObj = new object();
        private Type[] types;

        public TypeFinder(IAssemblyFinder assemblyFinder)
        {
            this.assemblyFinder = assemblyFinder;
            Logger = NullLogger.Instance;
        }

        public Type[] Find(Func<Type, bool> predicate)
        {
            return GetAllTypes().Where(predicate).ToArray();
        }

        public Type[] FindAll()
        {
            return GetAllTypes().ToArray();
        }

        private Type[] GetAllTypes()
        {
            if (types == null)
            {
                lock (syncObj)
                {
                    if (types == null)
                    {
                        types = CreateTypeList().ToArray();
                    }
                }
            }

            return types;
        }

        private List<Type> CreateTypeList()
        {
            var allTypes = new List<Type>();

            var assemblies = assemblyFinder.GetAllAssemblies().Distinct();

            foreach (var assembly in assemblies)
            {
                try
                {
                    Type[] typesInThisAssembly;

                    try
                    {
                        typesInThisAssembly = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        typesInThisAssembly = ex.Types;
                    }

                    if (typesInThisAssembly.IsNullOrEmpty())
                    {
                        continue;
                    }

                    allTypes.AddRange(typesInThisAssembly.Where(type => type != null));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                }
            }

            return allTypes;
        }
    }
}