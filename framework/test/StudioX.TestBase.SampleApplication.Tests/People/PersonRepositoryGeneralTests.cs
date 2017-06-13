using System.Linq;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Reflection;
using StudioX.TestBase.SampleApplication.EntityFramework.Repositories;
using StudioX.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.People
{
    public class PersonRepositoryGeneralTests : SampleApplicationTestBase
    {
        private readonly IRepository<Person> personRepository;

        public PersonRepositoryGeneralTests()
        {
            personRepository = Resolve<IRepository<Person>>();
        }

        [Fact]
        public void ShouldUseCustomBaseRepositoryClass()
        {
            (ProxyHelper.UnProxy(personRepository) is SampleApplicationEfRepositoryBase<Person>).ShouldBeTrue();
        }

        [Fact]
        public void ShouldDeleteEntityNotInContext()
        {
            var person = UsingDbContext(context => context.People.Single(p => p.Name == "Long"));
            personRepository.Delete(person);
            UsingDbContext(context => context.People.FirstOrDefault(p => p.Name == "Long")).IsDeleted.ShouldBe(true);
        }

        [Fact]
        public void TestUpdate()
        {
            var personInitial = UsingDbContext(context => context.People.Single(p => p.Name == "Long"));

            personRepository.Update(new Person
            {
                Id = personInitial.Id,
                Name = "Long-updated",
                ContactListId = personInitial.ContactListId
            });

            var personFinal = UsingDbContext(context => context.People.Single(p => p.Id == personInitial.Id));
            personFinal.Name.ShouldBe("Long-updated");
        }

        [Fact]
        public void ShouldInsertNewEntity()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var contactList = UsingDbContext(context => context.ContactLists.First());

                var person = new Person
                {
                    Name = "test-person",
                    ContactListId = contactList.Id
                };

                personRepository.Insert(person);

                person.IsTransient().ShouldBeTrue();

                uow.Complete();

                person.IsTransient().ShouldBeFalse();
            }
        }
    }
}