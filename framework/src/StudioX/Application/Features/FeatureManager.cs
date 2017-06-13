using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using StudioX.Dependency;

namespace StudioX.Application.Features
{
    /// <summary>
    /// Implements <see cref="IFeatureManager"/>.
    /// </summary>
    internal class FeatureManager : FeatureDefinitionContextBase, IFeatureManager, ISingletonDependency
    {
        private readonly IIocManager iocManager;
        private readonly IFeatureConfiguration featureConfiguration;

        public FeatureManager(IIocManager iocManager, IFeatureConfiguration featureConfiguration)
        {
            this.iocManager = iocManager;
            this.featureConfiguration = featureConfiguration;
        }

        public void Initialize()
        {
            foreach (var providerType in featureConfiguration.Providers)
            {
                using (var provider = CreateProvider(providerType))
                {
                    provider.Object.SetFeatures(this);
                }
            }

            Features.AddAllFeatures();
        }

        public Feature Get(string name)
        {
            var feature = GetOrNull(name);
            if (feature == null)
            {
                throw new StudioXException("There is no feature with name: " + name);
            }

            return feature;
        }

        public IReadOnlyList<Feature> GetAll()
        {
            return Features.Values.ToImmutableList();
        }

        private IDisposableDependencyObjectWrapper<FeatureProvider> CreateProvider(Type providerType)
        {
            return iocManager.ResolveAsDisposable<FeatureProvider>(providerType);
        }
    }
}
