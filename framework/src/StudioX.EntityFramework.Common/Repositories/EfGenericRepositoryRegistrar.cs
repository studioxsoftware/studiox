using System;
using System.Reflection;
using StudioX.Dependency;
using StudioX.Domain.Entities;
using StudioX.Domain.Repositories;
using StudioX.Reflection.Extensions;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;

namespace StudioX.EntityFramework.Repositories
{
    public class EfGenericRepositoryRegistrar : IEfGenericRepositoryRegistrar, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IDbContextEntityFinder dbContextEntityFinder;

        public EfGenericRepositoryRegistrar(IDbContextEntityFinder dbContextEntityFinder)
        {
            this.dbContextEntityFinder = dbContextEntityFinder;
            Logger = NullLogger.Instance;
        }

        public void RegisterForDbContext(
            Type dbContextType, 
            IIocManager iocManager, 
            AutoRepositoryTypesAttribute defaultAutoRepositoryTypesAttribute)
        {
            var autoRepositoryAttr = dbContextType.GetTypeInfo().GetSingleAttributeOrNull<AutoRepositoryTypesAttribute>() ?? defaultAutoRepositoryTypesAttribute;

            RegisterForDbContext(
                dbContextType,
                iocManager,
                autoRepositoryAttr.RepositoryInterface,
                autoRepositoryAttr.RepositoryInterfaceWithPrimaryKey,
                autoRepositoryAttr.RepositoryImplementation,
                autoRepositoryAttr.RepositoryImplementationWithPrimaryKey
            );

            if (autoRepositoryAttr.WithDefaultRepositoryInterfaces)
            {
                RegisterForDbContext(
                    dbContextType,
                    iocManager,
                    defaultAutoRepositoryTypesAttribute.RepositoryInterface,
                    defaultAutoRepositoryTypesAttribute.RepositoryInterfaceWithPrimaryKey,
                    autoRepositoryAttr.RepositoryImplementation,
                    autoRepositoryAttr.RepositoryImplementationWithPrimaryKey
                );
            }
        }

        private void RegisterForDbContext(
            Type dbContextType, 
            IIocManager iocManager,
            Type repositoryInterface,
            Type repositoryInterfaceWithPrimaryKey,
            Type repositoryImplementation,
            Type repositoryImplementationWithPrimaryKey)
        {
            foreach (var entityTypeInfo in dbContextEntityFinder.GetEntityTypeInfos(dbContextType))
            {
                var primaryKeyType = EntityHelper.GetPrimaryKeyType(entityTypeInfo.EntityType);
                if (primaryKeyType == typeof(int))
                {
                    var genericRepositoryType = repositoryInterface.MakeGenericType(entityTypeInfo.EntityType);
                    if (!iocManager.IsRegistered(genericRepositoryType))
                    {
                        var implType = repositoryImplementation.GetGenericArguments().Length == 1
                            ? repositoryImplementation.MakeGenericType(entityTypeInfo.EntityType)
                            : repositoryImplementation.MakeGenericType(entityTypeInfo.DeclaringType,
                                entityTypeInfo.EntityType);

                        iocManager.IocContainer.Register(
                            Component
                                .For(genericRepositoryType)
                                .ImplementedBy(implType)
                                .Named(Guid.NewGuid().ToString("N"))
                                .LifestyleTransient()
                        );
                    }
                }

                var genericRepositoryTypeWithPrimaryKey = repositoryInterfaceWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType,primaryKeyType);
                if (!iocManager.IsRegistered(genericRepositoryTypeWithPrimaryKey))
                {
                    var implType = repositoryImplementationWithPrimaryKey.GetGenericArguments().Length == 2
                        ? repositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType, primaryKeyType)
                        : repositoryImplementationWithPrimaryKey.MakeGenericType(entityTypeInfo.DeclaringType, entityTypeInfo.EntityType, primaryKeyType);

                    iocManager.IocContainer.Register(
                        Component
                            .For(genericRepositoryTypeWithPrimaryKey)
                            .ImplementedBy(implType)
                            .Named(Guid.NewGuid().ToString("N"))
                            .LifestyleTransient()
                    );
                }
            }
        }
    }
}