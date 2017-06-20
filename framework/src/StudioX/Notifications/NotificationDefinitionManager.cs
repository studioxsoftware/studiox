using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using StudioX.Application.Features;
using StudioX.Authorization;
using StudioX.Collections.Extensions;
using StudioX.Dependency;

namespace StudioX.Notifications
{
    /// <summary>
    ///     Implements <see cref="INotificationDefinitionManager" />.
    /// </summary>
    internal class NotificationDefinitionManager : INotificationDefinitionManager, ISingletonDependency
    {
        private readonly INotificationConfiguration configuration;
        private readonly IocManager iocManager;

        private readonly IDictionary<string, NotificationDefinition> notificationDefinitions;

        public NotificationDefinitionManager(
            IocManager iocManager,
            INotificationConfiguration configuration)
        {
            this.configuration = configuration;
            this.iocManager = iocManager;

            notificationDefinitions = new Dictionary<string, NotificationDefinition>();
        }

        public void Initialize()
        {
            var context = new NotificationDefinitionContext(this);

            foreach (var providerType in configuration.Providers)
            {
                iocManager.RegisterIfNot(providerType, DependencyLifeStyle.Transient); //TODO: Needed?
                using (var provider = iocManager.ResolveAsDisposable<NotificationProvider>(providerType))
                {
                    provider.Object.SetNotifications(context);
                }
            }
        }

        public void Add(NotificationDefinition notificationDefinition)
        {
            if (notificationDefinitions.ContainsKey(notificationDefinition.Name))
            {
                throw new StudioXInitializationException(
                    "There is already a notification definition with given name: " + notificationDefinition.Name +
                    ". Notification names must be unique!");
            }

            notificationDefinitions[notificationDefinition.Name] = notificationDefinition;
        }

        public NotificationDefinition Get(string name)
        {
            var definition = GetOrNull(name);
            if (definition == null)
            {
                throw new StudioXException("There is no notification definition with given name: " + name);
            }

            return definition;
        }

        public NotificationDefinition GetOrNull(string name)
        {
            return notificationDefinitions.GetOrDefault(name);
        }

        public IReadOnlyList<NotificationDefinition> GetAll()
        {
            return notificationDefinitions.Values.ToImmutableList();
        }

        public async Task<bool> IsAvailableAsync(string name, UserIdentifier user)
        {
            var notificationDefinition = GetOrNull(name);
            if (notificationDefinition == null)
            {
                return true;
            }

            if (notificationDefinition.FeatureDependency != null)
            {
                using (var featureDependencyContext = iocManager.ResolveAsDisposable<FeatureDependencyContext>())
                {
                    featureDependencyContext.Object.TenantId = user.TenantId;

                    if (
                        !await notificationDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext.Object))
                    {
                        return false;
                    }
                }
            }

            if (notificationDefinition.PermissionDependency != null)
            {
                using (var permissionDependencyContext = iocManager.ResolveAsDisposable<PermissionDependencyContext>())
                {
                    permissionDependencyContext.Object.User = user;

                    if (
                        !await notificationDefinition.PermissionDependency.IsSatisfiedAsync(
                            permissionDependencyContext.Object))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<IReadOnlyList<NotificationDefinition>> GetAllAvailableAsync(UserIdentifier user)
        {
            var availableDefinitions = new List<NotificationDefinition>();

            using (var permissionDependencyContext = iocManager.ResolveAsDisposable<PermissionDependencyContext>())
            {
                permissionDependencyContext.Object.User = user;

                using (var featureDependencyContext = iocManager.ResolveAsDisposable<FeatureDependencyContext>())
                {
                    featureDependencyContext.Object.TenantId = user.TenantId;

                    foreach (var notificationDefinition in GetAll())
                    {
                        if (notificationDefinition.PermissionDependency != null &&
                            !await notificationDefinition.PermissionDependency.IsSatisfiedAsync(
                                permissionDependencyContext.Object))
                        {
                            continue;
                        }

                        if (user.TenantId.HasValue &&
                            notificationDefinition.FeatureDependency != null &&
                            !await notificationDefinition.FeatureDependency.IsSatisfiedAsync(
                                featureDependencyContext.Object))
                        {
                            continue;
                        }

                        availableDefinitions.Add(notificationDefinition);
                    }
                }
            }

            return availableDefinitions.ToImmutableList();
        }
    }
}