using System.Linq;
using System.Threading.Tasks;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.People
{
    public class PersonRepositorySoftDeleteTests : SampleApplicationTestBase
    {
        private readonly IRepository<Person> personRepository;

        public PersonRepositorySoftDeleteTests()
        {
            personRepository = Resolve<IRepository<Person>>();
        }

        [Fact]
        public void ShouldNotRetrieveSoftDeletedsAsDefault()
        {
            personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false);
        }

        [Fact]
        public void ShouldRetriveSoftDeletedsIfFilterIsDisabled()
        {
            var uowManager = Resolve<IUnitOfWorkManager>();
            using (var ouw = uowManager.Begin())
            {
                using (uowManager.Current.DisableFilter(StudioXDataFilters.SoftDelete))
                {
                    personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(true); //Getting deleted people
                }

                ouw.Complete();
            }
        }

        [Fact]
        public void ShouldDisableAndEnableFiltersForSoftDelete()
        {
            var uowManager = Resolve<IUnitOfWorkManager>();
            using (var ouw = uowManager.Begin())
            {
                personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false); //not getting deleted people since soft-delete is enabled by default

                using (uowManager.Current.DisableFilter(StudioXDataFilters.SoftDelete))
                {
                    personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(true); //getting deleted people

                    using (uowManager.Current.EnableFilter(StudioXDataFilters.SoftDelete)) //re-enabling filter
                    {
                        personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false); //not getting deleted people

                        using (uowManager.Current.EnableFilter(StudioXDataFilters.SoftDelete)) //enabling filter has no effect since it's already enabed
                        {
                            personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false); //not getting deleted people
                        }

                        personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false); //not getting deleted people
                    }

                    personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(true); //getting deleted people
                }

                personRepository.GetAllList().Any(p => p.Name == "emre").ShouldBe(false); //not getting deleted people

                ouw.Complete();
            }
        }

        [Fact]
        public async Task ShouldSetDeletionAuditInformations()
        {
            const long userId = 42;
            StudioXSession.UserId = userId;

            var uowManager = Resolve<IUnitOfWorkManager>();

            //Get an entity to delete
            var personToBeDeleted = (await personRepository.GetAllListAsync()).FirstOrDefault();
            personToBeDeleted.ShouldNotBe(null);

            //Deletion audit properties should be null since it's not deleted yet
            personToBeDeleted.IsDeleted.ShouldBe(false);
            personToBeDeleted.DeletionTime.ShouldBe(null);
            personToBeDeleted.DeleterUserId.ShouldBe(null);
            
            //Delete it
            await personRepository.DeleteAsync(personToBeDeleted.Id);

            //Check if it's deleted
            (await personRepository.FirstOrDefaultAsync(personToBeDeleted.Id)).ShouldBe(null);

            //Get deleted entity again and check audit informations
            using (var ouw = uowManager.Begin())
            {
                using (uowManager.Current.DisableFilter(StudioXDataFilters.SoftDelete))
                {
                    personToBeDeleted = await personRepository.FirstOrDefaultAsync(personToBeDeleted.Id);
                    personToBeDeleted.ShouldNotBe(null);

                    //Deletion audit properties should be set
                    personToBeDeleted.IsDeleted.ShouldBe(true);
                    personToBeDeleted.DeletionTime.ShouldNotBe(null);
                    personToBeDeleted.DeleterUserId.ShouldBe(userId);
                }

                ouw.Complete();
            }
        }
    }
}
