using System;
using System.Collections.Generic;
using StudioX.Collections.Extensions;
using StudioX.Dependency;

namespace StudioX.Resources.Embedded
{
    public class EmbeddedResourceManager : IEmbeddedResourceManager, ISingletonDependency
    {
        private readonly IEmbeddedResourcesConfiguration configuration;
        private readonly Lazy<Dictionary<string, EmbeddedResourceItem>> resources;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EmbeddedResourceManager(IEmbeddedResourcesConfiguration configuration)
        {
            this.configuration = configuration;
            resources = new Lazy<Dictionary<string, EmbeddedResourceItem>>(
                CreateResourcesDictionary,
                true
            );
        }

        /// <inheritdoc/>
        public EmbeddedResourceItem GetResource(string fullPath)
        {
            return resources.Value.GetOrDefault(EmbeddedResourcePathHelper.NormalizePath(fullPath));
        }

        private Dictionary<string, EmbeddedResourceItem> CreateResourcesDictionary()
        {
            var resources = new Dictionary<string, EmbeddedResourceItem>(StringComparer.OrdinalIgnoreCase);

            foreach (var source in configuration.Sources)
            {
                source.AddResources(resources);
            }

            return resources;
        }
    }
}