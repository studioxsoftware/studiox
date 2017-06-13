using System;
using System.Linq;
using StudioX.Configuration.Startup;
using StudioX.Domain.Repositories;
using StudioX.TestBase.SampleApplication.Crm;
using StudioX.TestBase.SampleApplication.Messages;
using StudioX.Timing;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Auditing
{
    public class AuditedEntityTests: SampleApplicationTestBase
    {
        private readonly IRepository<Message> messageRepository;
        private readonly IRepository<Company> companyRepository;

        public AuditedEntityTests()
        {
            messageRepository = Resolve<IRepository<Message>>();
            companyRepository = Resolve<IRepository<Company>>();
        }

        [Fact]
        public void ShouldWriteAuditProperties()
        {
            //Arrange
            StudioXSession.TenantId = 1;
            StudioXSession.UserId = 2;

            //Act: Create a new entity
            var createdMessage = messageRepository.Insert(new Message(StudioXSession.TenantId, "test message 1"));

            //Assert: Check creation properties
            createdMessage.CreatorUserId.ShouldBe(StudioXSession.UserId);
            createdMessage.CreationTime.ShouldBeGreaterThan(Clock.Now.Subtract(TimeSpan.FromSeconds(10)));
            createdMessage.CreationTime.ShouldBeLessThan(Clock.Now.Add(TimeSpan.FromSeconds(10)));

            //Act: Select the same entity
            var selectedMessage = messageRepository.Get(createdMessage.Id);

            //Assert: Select should not change audit properties
            selectedMessage.ShouldBe(createdMessage); //They should be same since Entity class overrides == operator.

            selectedMessage.CreationTime.ShouldBe(createdMessage.CreationTime);
            selectedMessage.CreatorUserId.ShouldBe(createdMessage.CreatorUserId);

            selectedMessage.LastModifierUserId.ShouldBeNull();
            selectedMessage.LastModificationTime.ShouldBeNull();

            selectedMessage.IsDeleted.ShouldBeFalse();
            selectedMessage.DeleterUserId.ShouldBeNull();
            selectedMessage.DeletionTime.ShouldBeNull();

            //Act: Update the entity
            selectedMessage.Text = "test message 1 - updated";
            messageRepository.Update(selectedMessage);

            //Assert: Modification properties should be changed
            selectedMessage.LastModifierUserId.ShouldBe(StudioXSession.UserId);
            selectedMessage.LastModificationTime.ShouldNotBeNull();
            selectedMessage.LastModificationTime.Value.ShouldBeGreaterThan(Clock.Now.Subtract(TimeSpan.FromSeconds(10)));
            selectedMessage.LastModificationTime.Value.ShouldBeLessThan(Clock.Now.Add(TimeSpan.FromSeconds(10)));

            //Act: Delete the entity
            messageRepository.Delete(selectedMessage);

            //Assert: Deletion audit properties should be set
            selectedMessage.IsDeleted.ShouldBe(true);
            selectedMessage.DeleterUserId.ShouldBe(StudioXSession.UserId);
            selectedMessage.DeletionTime.ShouldNotBeNull();
            selectedMessage.DeletionTime.Value.ShouldBeGreaterThan(Clock.Now.Subtract(TimeSpan.FromSeconds(10)));
            selectedMessage.DeletionTime.Value.ShouldBeLessThan(Clock.Now.Add(TimeSpan.FromSeconds(10)));
        }

        [Fact]
        public void ShouldNotSetAuditUserPropertiesOfHostEntitiesByTenantUser()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            //Login as host
            StudioXSession.TenantId = null;
            StudioXSession.UserId = 42;

            //Get a company to modify
            var company = companyRepository.GetAllList().First();
            company.LastModifierUserId.ShouldBeNull(); //initial value

            //Modify the company
            company.Name = company.Name + "1";
            companyRepository.Update(company);

            //LastModifierUserId should be set
            company.LastModifierUserId.ShouldBe(42);

            //Login as a tenant
            StudioXSession.TenantId = 1;
            StudioXSession.UserId = 43;

            //Get the same company to modify
            company = companyRepository.FirstOrDefault(company.Id);
            company.ShouldNotBeNull();
            company.LastModifierUserId.ShouldBe(42); //Previous user's id

            //Modify the company
            company.Name = company.Name + "1";
            companyRepository.Update(company);

            //LastModifierUserId should set to null since a tenant changing a host entity
            company.LastModifierUserId.ShouldBe(null);
        }
    }
}
