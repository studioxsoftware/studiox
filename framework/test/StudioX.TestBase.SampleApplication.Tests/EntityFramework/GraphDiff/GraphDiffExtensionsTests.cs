using StudioX.Configuration.Startup;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.EntityFramework.GraphDiff.Extensions;
using StudioX.TestBase.SampleApplication.ContacLists;
using StudioX.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.EntityFramework.GraphDiff
{
    public class GraphDiffExtensionsTests : SampleApplicationTestBase
    {
        private readonly IRepository<ContactList> contactListRepository;
        private readonly IRepository<Person> peopleRepository;

        public GraphDiffExtensionsTests()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            StudioXSession.TenantId = 3;

            contactListRepository = Resolve<IRepository<ContactList>>();
            peopleRepository = Resolve<IRepository<Person>>();
        }

        [Fact]
        public void NewEntityShouldBeAddedWithNavigationProperties()
        {
            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            using (var unitOfWork = unitOfWorkManager.Begin())
            {
                var list1 = contactListRepository.FirstOrDefault(list => list.Name == "List-1 of Tenant-3");

                var davidDoe = new Person { Name = "David Doe", ContactList = new ContactList {Id = list1.Id} };
                davidDoe = peopleRepository.AttachGraph(davidDoe);
                unitOfWorkManager.Current.SaveChanges();

                davidDoe.Id.ShouldNotBeNull();
                davidDoe.Id.ShouldNotBe(default(int));
                davidDoe.ContactListId.ShouldBe(list1.Id); //New entity should be attached with it's navigation property

                unitOfWork.Complete();
            }
        }

        [Fact]
        public void DisattachedEntityCanBeAttachedToUpdateCorrespondingExistingEntityWithNavigationProperties()
        {
            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            using (var unitOfWork = unitOfWorkManager.Begin())
            {
                var list1 = contactListRepository.FirstOrDefault(list => list.Name == "List-1 of Tenant-3");
                var list2 = contactListRepository.FirstOrDefault(list => list.Name == "List-2 of Tenant-3");
                var johnDoeFromContext = peopleRepository.FirstOrDefault(p => p.Name == "John Doe");

                unitOfWorkManager.Current.SaveChanges();

                johnDoeFromContext.ContactListId.ShouldBe(list1.Id); //Ensure that johnDoe is in list1 now

                var johnDoeDisattached = new Person {
                    Id = johnDoeFromContext.Id,
                    Name = "John Doe Junior",
                    ContactList = new ContactList { Id = list2.Id }
                };

                //As a result of graph attachment, we should get old entity with UPDATED nav property (EF6 would create a new entity as it's disattached);
                var johnDoeAfterBeeingAttached = peopleRepository.AttachGraph(johnDoeDisattached);
                unitOfWorkManager.Current.SaveChanges();

                johnDoeAfterBeeingAttached.Id.ShouldBe(johnDoeFromContext.Id); //As entity was detached (but not deleted), it should be updated (not re-created)
                johnDoeAfterBeeingAttached.Name.ShouldBe("John Doe Junior"); //As entity was detached (but not deleted), it should be updated (not re-created)
                johnDoeAfterBeeingAttached.ContactListId.ShouldBe(list2.Id); //Entity should be attached with it's navigation property

                unitOfWork.Complete();
            }
        }
    }
}
