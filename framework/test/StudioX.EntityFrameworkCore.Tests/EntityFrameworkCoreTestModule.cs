using System;
using System.Transactions;
using StudioX.Domain.Repositories;
using StudioX.EntityFrameworkCore.Tests.Domain;
using StudioX.EntityFrameworkCore.Tests.Ef;
using StudioX.Modules;
using StudioX.TestBase;
using Castle.MicroKernel.Registration;
using Microsoft.EntityFrameworkCore;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Reflection.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace StudioX.EntityFrameworkCore.Tests
{
    [DependsOn(typeof(StudioXEntityFrameworkCoreModule), typeof(StudioXTestBaseModule))]
    public class EntityFrameworkCoreTestModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsolationLevel = IsolationLevel.Unspecified;

            //BloggingDbContext
            RegisterBloggingDbContextToSqliteInMemoryDb(IocManager);

            //SupportDbContext
            RegisterSupportDbContextToSqliteInMemoryDb(IocManager);

            //Custom repository
            Configuration.ReplaceService<IRepository<Post, Guid>>(() =>
            {
                IocManager.IocContainer.Register(
                    Component.For<IRepository<Post, Guid>, IPostRepository, PostRepository>()
                        .ImplementedBy<PostRepository>()
                        .LifestyleTransient()
                );
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EntityFrameworkCoreTestModule).GetAssembly());
        }

        private static void RegisterBloggingDbContextToSqliteInMemoryDb(IIocManager iocManager)
        {
            var builder = new DbContextOptionsBuilder<BloggingDbContext>();

            builder.ReplaceService<IEntityMaterializerSource, StudioXEntityMaterializerSource>();

            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            builder.UseSqlite(inMemorySqlite);

            iocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<BloggingDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );

            inMemorySqlite.Open();
            new BloggingDbContext(builder.Options).Database.EnsureCreated();
        }

        private static void RegisterSupportDbContextToSqliteInMemoryDb(IIocManager iocManager)
        {
            var builder = new DbContextOptionsBuilder<SupportDbContext>();

            builder.ReplaceService<IEntityMaterializerSource, StudioXEntityMaterializerSource>();

            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            builder.UseSqlite(inMemorySqlite);

            iocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<SupportDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );

            inMemorySqlite.Open();
            new SupportDbContext(builder.Options).Database.EnsureCreated();
        }
    }
}