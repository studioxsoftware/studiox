using System;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.TestBase.SampleApplication.EntityFramework;
using StudioX.TestBase.SampleApplication.People;
using EntityFramework.DynamicFilters;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.People
{
    public class TwoDbContextUowTests : SampleApplicationTestBase
    {
        private readonly IRepository<Person> personRepository;
        private readonly IRepository<SecondDbContextEntity> secondDbContextEntityRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public TwoDbContextUowTests()
        {
            personRepository = Resolve<IRepository<Person>>();
            secondDbContextEntityRepository = Resolve<IRepository<SecondDbContextEntity>>();
            unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        //[Fact] //TODO: Not working since Effort does not support multiple db context!
        public async Task ShouldTwoDbContextShareSameTransaction()
        {
            var personInitial = UsingDbContext(context => context.People.Single(p => p.Name == "Long"));

            using (var uow = unitOfWorkManager.Begin())
            {
                await personRepository.UpdateAsync(new Person
                {
                    Id = personInitial.Id,
                    Name = "Long-updated",
                    ContactListId = personInitial.ContactListId
                });

                await secondDbContextEntityRepository.InsertAsync(new SecondDbContextEntity {Name = "test1"});

                await uow.CompleteAsync();
            }

            var personFinal = UsingDbContext(context => context.People.Single(p => p.Id == personInitial.Id));
            personFinal.Name.ShouldBe("Long-updated");

            using (var secondContext = Resolve<SecondDbContext>())
            {
                secondContext.DisableAllFilters();
                secondContext.SecondDbContextEntities.Count(e => e.Name == "test1").ShouldBe(1);
            }
        }
    }
}