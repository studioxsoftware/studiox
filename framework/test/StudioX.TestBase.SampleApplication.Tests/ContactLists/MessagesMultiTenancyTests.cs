using System;
using System.Linq;
using StudioX.Configuration.Startup;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.TestBase.SampleApplication.Messages;
using StudioX.TestBase.SampleApplication.Tests.People;
using StudioX.Timing;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.ContactLists
{
    public class MessagesMultiTenancyTests : SampleApplicationTestBase
    {
        private readonly IRepository<Message> messageRepository;

        public MessagesMultiTenancyTests()
        {
            messageRepository = Resolve<IRepository<Message>>();
        }

        protected override void CreateInitialData()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            base.CreateInitialData();
        }

        [Fact]
        public void EntityAuditPropertyTestsCrossTenantUser()
        {
            StudioXSession.TenantId = null;
            StudioXSession.UserId = 999;

            Message tenant1MessageNew;

            //Can not get tenant's data from host user
            var tenant1Message1 = messageRepository.FirstOrDefault(m => m.Text == "tenant-1-message-1");
            tenant1Message1.ShouldBeNull();

            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            using (var unitOfWork = unitOfWorkManager.Begin())
            {
                //Should start UOW with TenantId in the session
                unitOfWorkManager.Current.GetTenantId().ShouldBeNull();

                using (unitOfWorkManager.Current.SetTenantId(1))
                {
                    unitOfWorkManager.Current.GetTenantId().ShouldBe(1);

                    tenant1Message1 = messageRepository.FirstOrDefault(m => m.Text == "tenant-1-message-1");
                    tenant1Message1.ShouldNotBeNull(); //Can get tenant's data from host since we used SetTenantId()
                    tenant1Message1.LastModifierUserId.ShouldBeNull();
                    tenant1Message1.LastModificationTime.ShouldBeNull();

                    tenant1Message1.Text = "tenant-1-message-1-modified";

                    var tenant1Message2 = messageRepository.Single(m => m.Text == "tenant-1-message-2");
                    messageRepository.Delete(tenant1Message2);

                    tenant1MessageNew = messageRepository.Insert(new Message(1, "tenant-1-message-new"));
                }

                unitOfWork.Complete();
            }

            //Creation audit check
            tenant1MessageNew.IsTransient().ShouldBeFalse(); //It should be saved to database
            tenant1MessageNew.CreationTime.ShouldBeGreaterThan(Clock.Now.Subtract(TimeSpan.FromMinutes(1)));
            tenant1MessageNew.CreatorUserId.ShouldBeNull(); //It's not set since user in the StudioXSession is not that tenant's user!

            //Modification audit check
            tenant1Message1.LastModificationTime.ShouldNotBeNull(); //It's set since we modified Text
            tenant1Message1.LastModifierUserId.ShouldBeNull(); //It's not set since user in the StudioXSession is not that tenant's user!

            //Deletion audit check
            UsingDbContext(context =>
            {
                var tenant1Message2 = context.Messages.FirstOrDefault(m => m.Text == "tenant-1-message-2");
                tenant1Message2.ShouldNotBeNull();
                tenant1Message2.IsDeleted.ShouldBeTrue();
                tenant1Message2.DeletionTime.ShouldNotBeNull();
                tenant1Message2.DeleterUserId.ShouldBeNull(); //It's not set since user in the StudioXSession is not that tenant's user!
            });
        }

        [Fact]
        public void EntityAuditPropertyTestsSameTenantUser()
        {
            StudioXSession.TenantId = 1;
            StudioXSession.UserId = 999;

            Message tenant1MessageNew;

            //Can get tenant's data from same tenant user
            var tenant1Message1 = messageRepository.FirstOrDefault(m => m.Text == "tenant-1-message-1");
            tenant1Message1.ShouldNotBeNull();

            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            using (var unitOfWork = unitOfWorkManager.Begin())
            {
                //Should start UOW with TenantId in the session
                unitOfWorkManager.Current.GetTenantId().ShouldBe(1);

                tenant1Message1 = messageRepository.FirstOrDefault(m => m.Text == "tenant-1-message-1");
                tenant1Message1.ShouldNotBeNull();
                tenant1Message1.LastModifierUserId.ShouldBeNull();
                tenant1Message1.LastModificationTime.ShouldBeNull();

                tenant1Message1.Text = "tenant-1-message-1-modified";

                var tenant1Message2 = messageRepository.Single(m => m.Text == "tenant-1-message-2");
                messageRepository.Delete(tenant1Message2);

                tenant1MessageNew = messageRepository.Insert(new Message(1, "tenant-1-message-new"));

                unitOfWork.Complete();
            }

            //Creation audit check
            tenant1MessageNew.IsTransient().ShouldBeFalse(); //It should be saved to database
            tenant1MessageNew.CreationTime.ShouldBeGreaterThan(Clock.Now.Subtract(TimeSpan.FromMinutes(1)));
            tenant1MessageNew.CreatorUserId.ShouldBe(999); //It set since user in the StudioXSession is tenant's user!

            //Modification audit check
            tenant1Message1.LastModificationTime.ShouldNotBeNull(); //It's set since we modified Text
            tenant1Message1.LastModifierUserId.ShouldBe(999); //It set since user in the StudioXSession is tenant's user!

            //Deletion audit check
            UsingDbContext(context =>
            {
                var tenant1Message2 = context.Messages.FirstOrDefault(m => m.Text == "tenant-1-message-2");
                tenant1Message2.ShouldNotBeNull();
                tenant1Message2.IsDeleted.ShouldBeTrue();
                tenant1Message2.DeletionTime.ShouldNotBeNull();
                tenant1Message2.DeleterUserId.ShouldBe(999); //It set since user in the StudioXSession is tenant's user!
            });
        }

        [Fact]
        public void MayHaveTenantFilterTests()
        {
            StudioXSession.UserId = 1;

            //A tenant can reach to it's own data
            StudioXSession.TenantId = 1;
            messageRepository.Count().ShouldBe(2);
            messageRepository.GetAllList().Any(m => m.TenantId != StudioXSession.TenantId).ShouldBe(false);

            //Tenant 999999 has no data
            StudioXSession.TenantId = 999999;
            messageRepository.Count().ShouldBe(0);

            //Host can reach to it's own data (since MayHaveTenant filter is enabled by default)
            StudioXSession.TenantId = null;
            messageRepository.Count().ShouldBe(1);
            messageRepository.GetAllList().Any(m => m.TenantId != StudioXSession.TenantId).ShouldBe(false);

            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            using (var unitOfWork = unitOfWorkManager.Begin())
            {
                unitOfWorkManager.Current.GetTenantId().ShouldBe(null);
                
                //We can also set tenantId parameter's value without changing StudioXSession.TenantId
                using (unitOfWorkManager.Current.SetTenantId(1))
                {
                    unitOfWorkManager.Current.GetTenantId().ShouldBe(1);

                    //We should only get tenant 1's entities since we set tenantId to 1
                    messageRepository.Count().ShouldBe(2);
                    messageRepository.GetAllList().Any(m => m.TenantId != 1).ShouldBe(false);
                }

                unitOfWorkManager.Current.GetTenantId().ShouldBe(null);

                //We can disable the filter to get all entities of host and tenants
                using (unitOfWorkManager.Current.DisableFilter(StudioXDataFilters.MayHaveTenant))
                {
                    messageRepository.Count(m => m.TenantId != PersonRepositoryTestsForEntityChangeEvents.PersonHandler.FakeTenantId).ShouldBe(3);
                }

                unitOfWork.Complete();
            }
        }
    }
}