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

        private readonly IIocResolver _iocResolver;
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;

        public DefaultDbContextResolver(
            IIocResolver iocResolver,
            IDbContextTypeMatcher dbContextTypeMatcher)
        {
            _iocResolver = iocResolver;
            _dbContextTypeMatcher = dbContextTypeMatcher;
        }

        public TDbContext Resolve<TDbContext>(string connectionString, DbConnection existingConnection)
            where TDbContext : DbContext
        {
            var dbContextType = typeof(TDbContext);

            if (!dbContextType.GetTypeInfo().IsAbstract)
            {
                return _iocResolver.Resolve<TDbContext>(new
                {
                    options = CreateOptions<TDbContext>(connectionString, existingConnection)
                });
            }

            var concreteType = _dbContextTypeMatcher.GetConcreteType(dbContextType);

            return (TDbContext)_iocResolver.Resolve(concreteType, new
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
            if (_iocResolver.IsRegistered<IStudioXDbContextConfigurer<TDbContext>>())
            {
                var configuration = new StudioXDbContextConfiguration<TDbContext>(connectionString, existingConnection);

                configuration.DbContextOptions.ReplaceService<IEntityMaterializerSource, StudioXEntityMaterializerSource>();

                using (var configurer = _iocResolver.ResolveAsDisposable<IStudioXDbContextConfigurer<TDbContext>>())
                {
                    configurer.Object.Configure(configuration);
                }

                return configuration.DbContextOptions.Options;
            }

            if (_iocResolver.IsRegistered<DbContextOptions<TDbContext>>())
            {
                return _iocResolver.Resolve<DbContextOptions<TDbContext>>();
            }

            throw new StudioXException($"Could not resolve DbContextOptions for {typeof(TDbContext).AssemblyQualifiedName}.");
        }
    }
}