using System;
using System.Threading.Tasks;
using StudioX.Dapper.Repositories;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.EntityFrameworkCore.Dapper.Tests.Domain;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Entities;

using Shouldly;

using Xunit;

namespace StudioX.EntityFrameworkCore.Dapper.Tests.Tests
{
    public class TransactionTests : StudioXEfCoreDapperTestApplicationBase
    {
        private readonly IDapperRepository<Blog> blogDapperRepository;
        private readonly IRepository<Blog> blogRepository;
        private readonly IUnitOfWorkManager uowManager;

        public TransactionTests()
        {
            uowManager = Resolve<IUnitOfWorkManager>();
            blogRepository = Resolve<IRepository<Blog>>();
            blogDapperRepository = Resolve<IDapperRepository<Blog>>();
        }

        [Fact]
        public async Task ShouldRollbackTransactionOnFailure()
        {
            const string exceptionMessage = "This is a test exception!";

            string blogName = Guid.NewGuid().ToString("N");

            try
            {
                using (uowManager.Begin())
                {
                    await blogRepository.InsertAsync(
                        new Blog(blogName, $"http://{blogName}.com/")
                    );

                    throw new Exception(exceptionMessage);
                }
            }
            catch (Exception ex) when (ex.Message == exceptionMessage)
            {
            }

            blogRepository.FirstOrDefault(x => x.Name == blogName).ShouldBeNull();
        }

        [Fact]
        public void DapperAndEfCoreShouldWorkUderSameUnitOfWork()
        {
            Resolve<IEventBus>().Register<EntityCreatingEventData<Blog>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("SameUow");

                    throw new Exception("Uow Rollback");
                });

            try
            {
                using (IUnitOfWorkCompleteHandle uow = Resolve<IUnitOfWorkManager>().Begin())
                {
                    int blogId = blogDapperRepository.InsertAndGetId(new Blog("SameUow", "www.oguzhansoykan.com"));

                    Blog person = blogRepository.Get(blogId);

                    person.ShouldNotBeNull();

                    uow.Complete();
                }
            }
            catch (Exception exception)
            {
                //no handling.
            }

            blogDapperRepository.FirstOrDefault(x => x.Name == "SameUow").ShouldBeNull();
            blogRepository.FirstOrDefault(x => x.Name == "SameUow").ShouldBeNull();
        }

        [Fact]
        public async Task InlineSqlWithDapperShouldR0ollbackWhenUoWFails()
        {

            Resolve<IEventBus>().Register<EntityCreatingEventData<Blog>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("SameUow");
                });

            int blogId = 0;
            using (IUnitOfWorkCompleteHandle uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                blogId = blogDapperRepository.InsertAndGetId(new Blog("SameUow", "www.studiox.com"));

                Blog person = blogRepository.Get(blogId);

                person.ShouldNotBeNull();

                uow.Complete();
            }

            try
            {
                using (IUnitOfWorkCompleteHandle uow = Resolve<IUnitOfWorkManager>().Begin(new UnitOfWorkOptions {IsTransactional = true}))
                {
                    await blogDapperRepository.ExecuteAsync("Update Blogs Set Name = @name where Id =@id", new { id = blogId, name = "NewBlog" });

                    throw new Exception("uow rollback");

                    uow.Complete();
                }

            }
            catch (Exception exception)
            {
                //no handling.
            }

            blogDapperRepository.FirstOrDefault(x => x.Name == "NewBlog").ShouldBeNull();
            blogRepository.FirstOrDefault(x => x.Name == "NewBlog").ShouldBeNull();

            blogDapperRepository.FirstOrDefault(x=>x.Name == "SameUow").ShouldNotBeNull();
            blogRepository.FirstOrDefault(x=>x.Name == "SameUow").ShouldNotBeNull();
        }
    }
}
