using System;
using System.Collections.Generic;
using System.Reflection;
using System.Transactions;
using StudioX.Configuration.Startup;
using StudioX.Dapper;
using StudioX.Domain.Repositories;
using StudioX.EntityFrameworkCore.Dapper.Tests.Domain;
using StudioX.EntityFrameworkCore.Dapper.Tests.Ef;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.TestBase;
using Castle.MicroKernel.Registration;
using DapperExtensions.Sql;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace StudioX.EntityFrameworkCore.Dapper.Tests
{
    [DependsOn(
        typeof(StudioXEntityFrameworkCoreModule),
        typeof(StudioXDapperModule),
        typeof(StudioXTestBaseModule))]
    public class StudioXEfCoreDapperTestModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsolationLevel = IsolationLevel.Unspecified;

            DapperExtensions.DapperExtensions.SqlDialect = new SqliteDialect();
            
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
            var builder = new DbContextOptionsBuilder<BloggingDbContext>();

            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            builder.UseSqlite(inMemorySqlite);

            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<BloggingDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );

            inMemorySqlite.Open();
            new BloggingDbContext(builder.Options).Database.EnsureCreated();

            IocManager.RegisterAssemblyByConvention(typeof(StudioXEfCoreDapperTestModule).GetAssembly());

            DapperExtensions.DapperExtensions.SetMappingAssemblies(new List<Assembly> { typeof(StudioXEfCoreDapperTestModule).GetAssembly() });
        }
    }
}
