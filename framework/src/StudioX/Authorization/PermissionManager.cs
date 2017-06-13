using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using StudioX.Application.Features;
using StudioX.Collections.Extensions;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.MultiTenancy;
using StudioX.Runtime.Session;

namespace StudioX.Authorization
{
    /// <summary>
    /// Permission manager.
    /// </summary>
    internal class PermissionManager : PermissionDefinitionContextBase, IPermissionManager, ISingletonDependency
    {
        public IStudioXSession StudioXSession { get; set; }

        private readonly IIocManager iocManager;
        private readonly IAuthorizationConfiguration authorizationConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PermissionManager(
            IIocManager iocManager,
            IAuthorizationConfiguration authorizationConfiguration)
        {
            this.iocManager = iocManager;
            this.authorizationConfiguration = authorizationConfiguration;

            StudioXSession = NullStudioXSession.Instance;
        }

        public void Initialize()
        {
            foreach (var providerType in authorizationConfiguration.Providers)
            {
                using (var provider = iocManager.ResolveAsDisposable<AuthorizationProvider>(providerType))
                {
                    provider.Object.SetPermissions(this);
                }
            }

            Permissions.AddAllPermissions();
        }

        public Permission GetPermission(string name)
        {
            var permission = Permissions.GetOrDefault(name);
            if (permission == null)
            {
                throw new StudioXException("There is no permission with name: " + name);
            }

            return permission;
        }

        public IReadOnlyList<Permission> GetAllPermissions(bool tenancyFilter = true)
        {
            using (var featureDependencyContext = iocManager.ResolveAsDisposable<FeatureDependencyContext>())
            {
                var featureDependencyContextObject = featureDependencyContext.Object;
                return Permissions.Values
                    .WhereIf(tenancyFilter, p => p.MultiTenancySides.HasFlag(StudioXSession.MultiTenancySide))
                    .Where(p =>
                        p.FeatureDependency == null ||
                        StudioXSession.MultiTenancySide == MultiTenancySides.Host ||
                        p.FeatureDependency.IsSatisfied(featureDependencyContextObject)
                    ).ToImmutableList();
            }
        }

        public IReadOnlyList<Permission> GetAllPermissions(MultiTenancySides multiTenancySides)
        {
            using (var featureDependencyContext = iocManager.ResolveAsDisposable<FeatureDependencyContext>())
            {
                var featureDependencyContextObject = featureDependencyContext.Object;
                return Permissions.Values
                    .Where(p => p.MultiTenancySides.HasFlag(multiTenancySides))
                    .Where(p =>
                        p.FeatureDependency == null ||
                        StudioXSession.MultiTenancySide == MultiTenancySides.Host ||
                        (p.MultiTenancySides.HasFlag(MultiTenancySides.Host) &&
                         multiTenancySides.HasFlag(MultiTenancySides.Host)) ||
                        p.FeatureDependency.IsSatisfied(featureDependencyContextObject)
                    ).ToImmutableList();
            }
        }
    }
}