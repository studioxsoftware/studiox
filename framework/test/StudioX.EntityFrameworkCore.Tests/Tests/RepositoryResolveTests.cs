using System;
using System.Linq;
using System.Reflection;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.EntityFrameworkCore.Repositories;
using StudioX.EntityFrameworkCore.Tests.Domain;
using StudioX.EntityFrameworkCore.Tests.Ef;
using Shouldly;
using Xunit;

namespace StudioX.EntityFrameworkCore.Tests.Tests
{
    public class RepositoryResolveTests : EntityFrameworkCoreModuleTestBase
    {
        [Fact]
        public void ShouldResolveCustomRepositoryIfRegistered()
        {
            var postRepository = Resolve<IRepository<Post, Guid>>();

            postRepository.GetAllList().Any().ShouldBeTrue();

            Assert.Throws<Exception>(
                () => postRepository.Count()
            ).Message.ShouldBe("can not get count of posts");

            //Should also resolve by custom interface and implementation
            Resolve<IPostRepository>();
            Resolve<PostRepository>();
        }

        [Fact]
        public void ShouldResolveDefaultRepositoriesForSecondDbContext()
        {
            var repo1 = Resolve<IRepository<Ticket>>();
            var repo2 = Resolve<IRepository<Ticket, int>>();

            Assert.Throws<Exception>(
                () => repo1.Count()
            ).Message.ShouldBe("can not get count!");

            Assert.Throws<Exception>(
                () => repo2.Count()
            ).Message.ShouldBe("can not get count!");
        }

        [Fact]
        public void ShouldResolveCustomRepositoriesForSecondDbContext()
        {
            var repo1 = Resolve<ISupportRepository<Ticket>>();
            var repo2 = Resolve<ISupportRepository<Ticket, int>>();

            typeof(ISupportRepository<Ticket>).GetTypeInfo().IsInstanceOfType(repo1).ShouldBeTrue();
            typeof(ISupportRepository<Ticket, int>).GetTypeInfo().IsInstanceOfType(repo1).ShouldBeTrue();
            typeof(ISupportRepository<Ticket, int>).GetTypeInfo().IsInstanceOfType(repo2).ShouldBeTrue();

            Assert.Throws<Exception>(
                () => repo1.Count()
            ).Message.ShouldBe("can not get count!");

            Assert.Throws<Exception>(
                () => repo2.Count()
            ).Message.ShouldBe("can not get count!");

            var activeTickets = repo1.GetActiveList();
            activeTickets.Count.ShouldBe(1);
            activeTickets[0].IsActive.ShouldBeTrue();

            activeTickets = repo2.GetActiveList();
            activeTickets.Count.ShouldBe(1);
            activeTickets[0].IsActive.ShouldBeTrue();
        }

        [Fact]
        public void ShouldGetDbContext()
        {
            Resolve<IPostRepository>().GetDbContext().ShouldBeOfType<BloggingDbContext>();
        }

        [Fact]
        public void ShouldGetDbContext2()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                Resolve<IRepository<Blog>>().GetDbContext().ShouldBeOfType<BloggingDbContext>();

                uow.Complete();
            }
        }

        [Fact]
        public void ShouldGetDbContextFromSecondDbContext()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                Resolve<IRepository<Ticket>>().GetDbContext().ShouldBeOfType<SupportDbContext>();

                uow.Complete();
            }
        }

        [Fact]
        public void ShouldGetDbContextFromSecondDbContextWithCustomRepository()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                Resolve<ISupportRepository<Ticket>>().GetDbContext().ShouldBeOfType<SupportDbContext>();

                uow.Complete();
            }
        }
    }
}