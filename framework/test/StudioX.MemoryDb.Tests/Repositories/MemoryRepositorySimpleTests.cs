using StudioX.Domain.Entities;
using StudioX.Domain.Repositories;
using StudioX.MemoryDb.Repositories;
using NSubstitute;
using Shouldly;
using Xunit;

namespace StudioX.MemoryDb.Tests.Repositories
{
    public class MemoryRepositorySimpleTests
    {
        private readonly MemoryDatabase database;
        private readonly IRepository<MyEntity> repository;

        public MemoryRepositorySimpleTests()
        {
            database = new MemoryDatabase();

            var databaseProvider = Substitute.For<IMemoryDatabaseProvider>();
            databaseProvider.Database.Returns(database);

            repository = new MemoryRepository<MyEntity>(databaseProvider);

            //Testing Insert by creating initial data
            repository.Insert(new MyEntity("test-1"));
            repository.Insert(new MyEntity("test-2"));
            database.Set<MyEntity>().Count.ShouldBe(2);
        }

        [Fact]
        public void CountTest()
        {
            repository.Count().ShouldBe(2);
        }

        [Fact]
        public void DeleteTest()
        {
            var test1 = repository.FirstOrDefault(e => e.Name == "test-1");
            test1.ShouldNotBe(null);

            repository.Delete(test1);
            
            test1 = repository.FirstOrDefault(e => e.Name == "test-1");
            test1.ShouldBe(null);
        }

        public class MyEntity : Entity
        {
            public string Name { get; set; }

            public MyEntity()
            {
                
            }

            public MyEntity(string name)
            {
                Name = name;
            }
        }
    }
}
