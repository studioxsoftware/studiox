using System;
using System.Threading.Tasks;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace StudioX.EntityFrameworkCore.Tests.Tests
{
    //WE CAN NOT TEST TRANSACTIONS SINCE INMEMORY DB DOES NOT SUPPORT IT! TODO: Use SQLite
    public class TransactionTests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IUnitOfWorkManager uowManager;
        private readonly IRepository<Blog> blogRepository;

        public TransactionTests()
        {
            uowManager = Resolve<IUnitOfWorkManager>();
            blogRepository = Resolve<IRepository<Blog>>();
        }

        //[Fact] 
        public async Task ShouldRollbackTransactionOnFailure()
        {
            const string exceptionMessage = "This is a test exception!";

            var blogName = Guid.NewGuid().ToString("N");

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
            
            await UsingDbContextAsync(async context =>
            {
                var blog = await context.Blogs.FirstOrDefaultAsync(b => b.Name == blogName);
                blog.ShouldNotBeNull();
            });
        }
    }
}