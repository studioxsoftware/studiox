using System.Linq;

using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.EntityFramework.Extensions;
using StudioX.TestBase.SampleApplication.People;

using Shouldly;

using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.People
{
    public class PersonRepositoryGeneralNolockingTests : SampleApplicationTestBase
    {
        private readonly IRepository<Person> personRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public PersonRepositoryGeneralNolockingTests()
        {
            personRepository = Resolve<IRepository<Person>>();
            unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        //[Fact(Skip = "Skipped, since Effort.DbConnection does not provide Sql Text while interception time.")]
        public void ShouldNolockingWork()
        {
            using (IUnitOfWorkCompleteHandle uow = unitOfWorkManager.Begin())
            {
                Person person = personRepository.Nolocking(persons => persons.FirstOrDefault(x => x.Name == "Long"));
                person.ShouldNotBeNull();

                uow.Complete();
            }
        }
    }
}
