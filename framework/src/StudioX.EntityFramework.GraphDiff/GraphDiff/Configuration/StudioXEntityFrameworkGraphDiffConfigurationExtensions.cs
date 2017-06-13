using System.Collections.Generic;
using System.Linq;
using StudioX.Configuration.Startup;
using StudioX.EntityFramework.GraphDiff.Mapping;

namespace StudioX.EntityFramework.GraphDiff.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure StudioX.EntityFramework.GraphDiff module.
    /// </summary>
    public static class StudioXEntityFrameworkGraphDiffConfigurationExtensions
    {
        /// <summary>
        /// Used to configure StudioX.EntityFramework.GraphDiff module.
        /// </summary>
        public static IStudioXEntityFrameworkGraphDiffModuleConfiguration StudioXEfGraphDiff(this IModuleConfigurations configurations)
        {
            return configurations.StudioXConfiguration.Get<IStudioXEntityFrameworkGraphDiffModuleConfiguration>();
        }

        /// <summary>
        /// Used to provide a mappings for the StudioX.EntityFramework.GraphDiff module.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="entityMappings"></param>
        public static void UseMappings(this IStudioXEntityFrameworkGraphDiffModuleConfiguration configuration, IEnumerable<EntityMapping> entityMappings)
        {
            configuration.EntityMappings = entityMappings.ToList();
        }
    }
}
