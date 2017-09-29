using System;
using System.Collections.Generic;
using StudioX.Dependency;
using StudioX.Modules;

namespace StudioX.MemoryDb
{
    [DependsOn(typeof(StudioXKernelModule))]
    public class MemoryDatabase : ISingletonDependency
    {
        private readonly Dictionary<Type, object> sets;

        private readonly object syncObj = new object();

        public MemoryDatabase()
        {
            sets = new Dictionary<Type, object>();
        }

        public List<TEntity> Set<TEntity>()
        {
            var entityType = typeof(TEntity);

            lock (syncObj)
            {
                if (!sets.ContainsKey(entityType))
                {
                    sets[entityType] = new List<TEntity>();
                }

                return sets[entityType] as List<TEntity>;
            }
        }
    }
}