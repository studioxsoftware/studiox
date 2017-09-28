using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace StudioX.EntityFrameworkCore.Tests.Tests
{
    public class ParallelQueryingTests : EntityFrameworkCoreModuleTestBase
    {
        private readonly ParallelQueryExecuteDemo parallelQueryExecuteDemo;

        public ParallelQueryingTests()
        {
            parallelQueryExecuteDemo = Resolve<ParallelQueryExecuteDemo>();
        }

        //[Fact]
        public async Task ShouldRunParallelWithDifferentUnitOfWorks()
        {
            await parallelQueryExecuteDemo.RunAsync();
        }
    }

    public class ParallelQueryExecuteDemo : ITransientDependency
    {
        private readonly IRepository<Blog> blogRepository;

        public ParallelQueryExecuteDemo(IRepository<Blog> blogRepository)
        {
            this.blogRepository = blogRepository;
        }

        [UnitOfWork]
        public virtual async Task RunAsync()
        {
            const int threadCount = 32;

            var tasks = new List<Task<int>>();

            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(GetBlogCountAsync());
            }

            await Task.WhenAll(tasks.Cast<Task>().ToArray());

            foreach (var task in tasks)
            {
                task.Result.ShouldBeGreaterThan(0);
            }
        }

        [UnitOfWork(TransactionScopeOption.RequiresNew, false)]
        public virtual async Task<int> GetBlogCountAsync()
        {
            await Task.Delay(RandomHelper.GetRandom(0, 100));
            var result = await blogRepository.GetAll().CountAsync();
            await Task.Delay(RandomHelper.GetRandom(0, 100));
            return result;
        }
    }
}
