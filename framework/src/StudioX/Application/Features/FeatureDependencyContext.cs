using StudioX.Dependency;

namespace StudioX.Application.Features
{
    /// <summary>
    ///     Implementation of <see cref="IFeatureDependencyContext" />.
    /// </summary>
    public class FeatureDependencyContext : IFeatureDependencyContext, ITransientDependency
    {
        public int? TenantId { get; set; }

        /// <inheritdoc />
        public IIocResolver IocResolver { get; }

        /// <inheritdoc />
        public IFeatureChecker FeatureChecker { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FeatureDependencyContext" /> class.
        /// </summary>
        public FeatureDependencyContext(IIocResolver iocResolver, IFeatureChecker featureChecker)
        {
            IocResolver = iocResolver;
            FeatureChecker = featureChecker;
        }
    }
}