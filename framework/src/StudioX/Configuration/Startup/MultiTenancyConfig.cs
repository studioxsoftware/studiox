using StudioX.Collections;
using StudioX.MultiTenancy;

namespace StudioX.Configuration.Startup
{
    /// <summary>
    ///     Used to configure multi-tenancy.
    /// </summary>
    internal class MultiTenancyConfig : IMultiTenancyConfig
    {
        /// <summary>
        ///     Is multi-tenancy enabled?
        ///     Default value: false.
        /// </summary>
        public bool IsEnabled { get; set; }

        public ITypeList<ITenantResolveContributor> Resolvers { get; }

        public MultiTenancyConfig()
        {
            Resolvers = new TypeList<ITenantResolveContributor>();
        }
    }
}