using System;
using System.Linq;
using System.Linq.Expressions;
using StudioX.Dependency;
using StudioX.EntityFramework.GraphDiff.Configuration;
using RefactorThis.GraphDiff;

namespace StudioX.EntityFramework.GraphDiff.Mapping
{
    /// <summary>
    /// Used for resolving mappings for a GraphDiff repository extension methods
    /// </summary>
    public class EntityMappingManager : IEntityMappingManager, ITransientDependency
    {
        private readonly IStudioXEntityFrameworkGraphDiffModuleConfiguration moduleConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EntityMappingManager(IStudioXEntityFrameworkGraphDiffModuleConfiguration moduleConfiguration)
        {
            this.moduleConfiguration = moduleConfiguration;
        }

        /// <summary>
        /// Gets an entity mapping or null for a specified entity type
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Entity mapping or null if mapping doesn't exist</returns>
        public Expression<Func<IUpdateConfiguration<TEntity>, object>> GetEntityMappingOrNull<TEntity>()
        {
            var entityMapping = moduleConfiguration.EntityMappings.FirstOrDefault(m => m.EntityType == typeof(TEntity));
            var mappingExptession = entityMapping?.MappingExpression as Expression<Func<IUpdateConfiguration<TEntity>, object>>;
            return mappingExptession;
        }
    }
}