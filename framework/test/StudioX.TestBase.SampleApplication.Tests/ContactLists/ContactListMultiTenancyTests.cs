using System.Linq;
using StudioX.Configuration.Startup;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.TestBase.SampleApplication.ContacLists;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.ContactLists
{
    public class ContactListMultiTenancyTests : SampleApplicationTestBase
    {
        private readonly IRepository<ContactList> contactListRepository;
        private readonly IContactListAppService contactListAppService;

        public ContactListMultiTenancyTests()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            contactListRepository = Resolve<IRepository<ContactList>>();
            contactListAppService = Resolve<IContactListAppService>();
        }

        [Fact]
        public void MustHaveTenantFilter()
        {
            StudioXSession.UserId = 1;

            //A tenant can reach to it's own data
            StudioXSession.TenantId = 1;
            contactListRepository.GetAllList().Any(cl => cl.TenantId != StudioXSession.TenantId).ShouldBe(false);

            //A tenant can reach to it's own data
            StudioXSession.TenantId = 2;
            contactListRepository.GetAllList().Any(cl => cl.TenantId != StudioXSession.TenantId).ShouldBe(false);

            //Tenant 999999 has no data
            StudioXSession.TenantId = 999999;
            contactListRepository.GetAllList().Count.ShouldBe(0);

            //Host can reach to all tenant data (since MustHaveTenant filter is disabled for host as default)
            StudioXSession.TenantId = null;
            contactListRepository.GetAllList().Count.ShouldBe(4);

            //Host can filter tenant data if it wants
            contactListRepository.GetAllList().Count(t => t.TenantId == 1).ShouldBe(1);
            contactListRepository.GetAllList().Count(t => t.TenantId == 2).ShouldBe(1);
            contactListRepository.GetAllList().Count(t => t.TenantId == 999999).ShouldBe(0);

            //We can also set tenantId parameter's value without changing StudioXSession.TenantId
            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            using (var unitOfWork = unitOfWorkManager.Begin())
            {
                //Host can reach to all tenant data (since MustHaveTenant filter is disabled for host as default)
                contactListRepository.GetAllList().Count.ShouldBe(4);

                using (unitOfWorkManager.Current.EnableFilter(StudioXDataFilters.MustHaveTenant))
                {
                    //We can not get any entity since filter is enabled (even we're host)
                    contactListRepository.GetAllList().Count.ShouldBe(0);

                    //We're overriding filter parameter's value
                    unitOfWorkManager.Current.SetTenantId(1);

                    //We should only get tenant 1's entities since we set tenantId to 1
                    var contactLists = contactListRepository.GetAllList();
                    contactLists.Count.ShouldBe(1);
                    contactLists.Any(cl => cl.TenantId != 1).ShouldBe(false);
                }

                unitOfWork.Complete();
            }
        }

        [Fact]
        public void SettingSetTenantIdShouldEnableOrDisableMustHaveTenantFilter()
        {
            StudioXSession.TenantId = null;

            var unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            using (var unitOfWork = unitOfWorkManager.Begin())
            {
                //Host can reach to all tenant data (since MustHaveTenant filter is disabled for host as default)
                contactListRepository.GetAllList().Count.ShouldBe(4);

                unitOfWorkManager.Current.SetTenantId(1);
                //We should only get tenant 1's entities since we set tenantId to 1 (which automatically enables MustHaveTenant filter)
                var contactLists = contactListRepository.GetAllList();
                contactLists.Count.ShouldBe(1);
                contactLists.Any(cl => cl.TenantId != 1).ShouldBe(false);

                unitOfWorkManager.Current.SetTenantId(null);
                //Switched to host, which automatically disables MustHaveTenant filter
                contactListRepository.GetAllList().Count.ShouldBe(4);

                unitOfWork.Complete();
            }
        }

        [Fact]
        public void MustHaveTenantShouldWorkInAppService()
        {
            StudioXSession.TenantId = 3;
            StudioXSession.UserId = 3;

            var lists = contactListAppService.GetContactLists();
            lists.Count.ShouldBeGreaterThan(0);
        }
    }
}
