using StudioX.Dependency;
using StudioX.EntityFramework;
using StudioX.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace StudioX.EntityFrameworkCore
{
    public class DefaultDbContextResolver : IDbContextResolver, ITransientDependency
    {
        private static readonly MethodInfo CreateOptionsMethod = typeof(DefaultDbContextResolver).GetMethod("CreateOptions", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly IIocResolver iocResolver;
        private readonly IDbContextTypeMatcher dbContextTypeMatcher;

        public DefaultDbContextResolver(
            IIocResolver iocResolver,
            IDbContextTypeMatcher dbContextTypeMatcher)
        {
            this.iocResolver = iocResolver;
            this.dbContextTypeMatcher = dbContextTypeMatcher;
        }

        public TDbContext Resolve<TDbContext>(string connectionString, DbConnection existingConnection)
            where TDbContext : DbContext
        {
            var dbContextType = typeof(TDbContext);

            if (!dbContextType.GetTypeInfo().IsAbstract)
            {
                return iocResolver.Resolve<TDbContext>(new
                {
                    options = CreateOptions<TDbContext>(connectionString, existingConnection)
                });
            }

            var concreteType = dbContextTypeMatcher.GetConcreteType(dbContextType);

            return (TDbContext)iocResolver.Resolve(concreteType, new
            {
                options = CreateOptionsForType(concreteType, connectionString, existingConnection)
            });
        }

        private object CreateOptionsForType(Type dbContextType, string connectionString, DbConnection existingConnection)
        {
            return CreateOptionsMethod.MakeGenericMethod(dbContextType).Invoke(this, new object[] { connectionString, existingConnection });
        }

        protected virtual DbContextOptions<TDbContext> CreateOptions<TDbContext>([NotNull] string connectionString, [CanBeNull] DbConnection existingConnection) where TDbContext : DbContext
        {
            if (iocResolver.IsRegistered<IStudioXDbContextConfigurer<TDbContext>>())
            {
                var configuration = new StudioXDbContextConfiguration<TDbContext>(connectionString, existingConnection);

                configuration.DbContextOptions.ReplaceService<IEntityMaterializerSource, StudioXEntityMaterializerSource>();

                using (var configurer = iocResolver.ResolveAsDisposable<IStudioXDbContextConfigurer<TDbContext>>())
                {
                    configurer.Object.Configure(configuration);
                }

                return configuration.DbContextOptions.Options;
            }

            if (iocResolver.IsRegistered<DbContextOptions<TDbContext>>())
            {
                return iocResolver.Resolve<DbContextOptions<TDbContext>>();
            }

            throw new StudioXException($"Could not resolve DbContextOptions for {typeof(TDbContext).AssemblyQualifiedName}.");
        }
    }
}