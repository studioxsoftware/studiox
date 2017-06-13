using System;
using System.Reflection;
using StudioX.Dependency;
using StudioX.Domain.Entities;
using StudioX.Domain.Repositories;
using StudioX.Orm;
using StudioX.Reflection.Extensions;

namespace StudioX.EntityFramework
{
    public abstract class SecondaryOrmRegistrarBase : ISecondaryOrmRegistrar
    {
        private readonly IDbContextEntityFinder dbContextEntityFinder;
        private readonly Type dbContextType;

        protected SecondaryOrmRegistrarBase(Type dbContextType, IDbContextEntityFinder dbContextEntityFinder)
        {
            this.dbContextType = dbContextType;
            this.dbContextEntityFinder = dbContextEntityFinder;
        }

        public abstract string OrmContextKey { get; }

        public virtual void RegisterRepositories(IIocManager iocManager,
            AutoRepositoryTypesAttribute defaultRepositoryTypes)
        {
            var autoRepositoryAttr = dbContextType.GetTypeInfo()
                                         .GetSingleAttributeOrNull<AutoRepositoryTypesAttribute>()
                                     ?? defaultRepositoryTypes;

            foreach (var entityTypeInfo in dbContextEntityFinder.GetEntityTypeInfos(dbContextType))
            {
                var primaryKeyType = EntityHelper.GetPrimaryKeyType(entityTypeInfo.EntityType);
                if (primaryKeyType == typeof(int))
                {
                    var genericRepositoryType =
                        autoRepositoryAttr.RepositoryInterface.MakeGenericType(entityTypeInfo.EntityType);
                    if (!iocManager.IsRegistered(genericRepositoryType))
                    {
                        var implType = autoRepositoryAttr.RepositoryImplementation.GetTypeInfo().GetGenericArguments()
                                           .Length == 1
                            ? autoRepositoryAttr.RepositoryImplementation.MakeGenericType(entityTypeInfo.EntityType)
                            : autoRepositoryAttr.RepositoryImplementation.MakeGenericType(entityTypeInfo.DeclaringType,
                                entityTypeInfo.EntityType);

                        iocManager.Register(
                            genericRepositoryType,
                            implType,
                            DependencyLifeStyle.Transient
                        );
                    }
                }

                var genericRepositoryTypeWithPrimaryKey =
                    autoRepositoryAttr.RepositoryInterfaceWithPrimaryKey.MakeGenericType(entityTypeInfo.EntityType,
                        primaryKeyType);
                if (!iocManager.IsRegistered(genericRepositoryTypeWithPrimaryKey))
                {
                    var implType = autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.GetTypeInfo()
                                       .GetGenericArguments().Length == 2
                        ? autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.MakeGenericType(
                            entityTypeInfo.EntityType, primaryKeyType)
                        : autoRepositoryAttr.RepositoryImplementationWithPrimaryKey.MakeGenericType(
                            entityTypeInfo.DeclaringType, entityTypeInfo.EntityType, primaryKeyType);

                    iocManager.Register(
                        genericRepositoryTypeWithPrimaryKey,
                        implType,
                        DependencyLifeStyle.Transient
                    );
                }
            }
        }
    }
}