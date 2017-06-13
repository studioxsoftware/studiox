using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Application.Services.Dto;
using StudioX.Authorization;
using StudioX.Domain.Uow;
using StudioX.Runtime.Validation;
using StudioX.TestBase.SampleApplication.ContacLists;
using StudioX.TestBase.SampleApplication.People;
using StudioX.TestBase.SampleApplication.People.Dto;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.People
{
    public class PersonAppServiceTests : SampleApplicationTestBase
    {
        [Fact]
        public async Task ShouldInsertNewPerson()
        {
            var personAppService = Resolve<IPersonAppService>();

            ContactList contactList = null;
            int peopleCount = 0;

            await UsingDbContext(
                async context =>
                {
                    contactList = await context.ContactLists.FirstOrDefaultAsync();
                    peopleCount = await context.People.CountAsync();
                });

            await personAppService.CreatePersonAsync(
                new CreatePersonInput
                {
                    ContactListId = contactList.Id,
                    Name = "john"
                });

            await UsingDbContext(async context =>
            {
                (await context.People.FirstOrDefaultAsync(p => p.Name == "john")).ShouldNotBe(null);
                (await context.People.CountAsync()).ShouldBe(peopleCount + 1);
            });
        }

        [Fact]
        public async Task ShouldRollbackIfUowIsNotCompleted()
        {
            var personAppService = Resolve<IPersonAppService>();

            ContactList contactList = null;
            int peopleCount = 0;

            await UsingDbContext(
                async context =>
                {
                    contactList = await context.ContactLists.FirstOrDefaultAsync();
                    peopleCount = await context.People.CountAsync();
                });

            //CreatePersonAsync will use same UOW.
            using (var uow = LocalIocManager.Resolve<IUnitOfWorkManager>().Begin())
            {
                await personAppService.CreatePersonAsync(new CreatePersonInput { ContactListId = contactList.Id, Name = "john" });
                //await uow.CompleteAsync(); //It's intentionally removed from code to see roll-back
            }

            //john will not be added since uow is not completed (so, rolled back)
            await UsingDbContext(async context =>
            {
                (await context.People.FirstOrDefaultAsync(p => p.Name == "john")).ShouldBe(null);
                (await context.People.CountAsync()).ShouldBe(peopleCount);
            });
        }

        [Fact]
        public async Task ShouldNotInsertForInvalidInput()
        {
            var personAppService = Resolve<IPersonAppService>();

            await Assert.ThrowsAsync<StudioXValidationException>(async () => await personAppService.CreatePersonAsync(new CreatePersonInput { Name = null }));
        }

        [Fact]
        public void ShouldGetAllPeopleWithoutFilter()
        {
            var personAppService = Resolve<IPersonAppService>();

            var output = personAppService.GetPeople(new GetPeopleInput());
            output.Items.Count.ShouldBe(UsingDbContext(context => context.People.Count(p => !p.IsDeleted)));
            output.Items.FirstOrDefault(p => p.Name == "Long").ShouldNotBe(null);
        }

        [Fact]
        public void ShouldGetRelatedPeopleWithFilter()
        {
            var personAppService = Resolve<IPersonAppService>();

            var output = personAppService.GetPeople(new GetPeopleInput { NameFilter = "g" });
            output.Items.FirstOrDefault(p => p.Name == "Long").ShouldNotBe(null);
            output.Items.All(p => p.Name.Contains("g")).ShouldBe(true);
        }

        [Fact]
        public async Task ShouldDeletePerson()
        {
            //Arrange

            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync("CanDeletePerson")
                .Returns(
                    async info =>
                    {
                        await Task.Delay(10);
                        return true;
                    });

            LocalIocManager.IocContainer.Register(
                Component.For<IPermissionChecker>().Instance(permissionChecker).IsDefault()
            );

            var personAppService = Resolve<IPersonAppService>();

            StudioXSession.UserId = 1;

            var Long = await UsingDbContextAsync(async context => await context.People.SingleAsync(p => p.Name == "Long"));

            //Act

            await personAppService.DeletePerson(new EntityDto(Long.Id));

            //Assert

            (await UsingDbContextAsync(async context => await context.People.FirstOrDefaultAsync(p => p.Name == "Long"))).IsDeleted.ShouldBe(true);
        }

        [Fact]
        public async Task ShouldNotDeletePersonIfUnAuthorized()
        {
            //Arrange

            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync("CanDeletePerson")
                .Returns(async info =>
                {
                    await Task.Delay(10);
                    return false;
                });

            LocalIocManager.IocContainer.Register(
                Component.For<IPermissionChecker>().Instance(permissionChecker).IsDefault()
            );

            var personAppService = Resolve<IPersonAppService>();

            StudioXSession.UserId = 1;
            
            var Long = await UsingDbContextAsync(async context => await context.People.SingleAsync(p => p.Name == "Long"));

            //Act & Assert

            await Assert.ThrowsAsync<StudioXAuthorizationException>(async () =>
            {
                await personAppService.DeletePerson(new EntityDto(Long.Id));
            });
        }

        [Fact]
        public void TestTestPrimitiveMethod()
        {
            var personAppService = Resolve<IPersonAppService>();

            personAppService.TestPrimitiveMethod(42, "adana", new EntityDto(7)).ShouldBe("42#adana#7");
        }
    }
}
