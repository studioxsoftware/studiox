using System.Transactions;
using StudioX.Configuration.Startup;
using StudioX.Domain.Uow;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Uow
{
    public class UnitOfWorkNestedTests : SampleApplicationTestBase
    {
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public UnitOfWorkNestedTests()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public void ShouldCopyFiltersToNestedUow()
        {
            StudioXSession.TenantId.ShouldBe(null);

            using (var outerUow = unitOfWorkManager.Begin())
            {
                unitOfWorkManager.Current.GetTenantId().ShouldBe(null);

                using (unitOfWorkManager.Current.SetTenantId(1))
                {
                    unitOfWorkManager.Current.GetTenantId().ShouldBe(1);

                    using (var nestedUow = unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                    {
                        StudioXSession.TenantId.ShouldBe(null);
                        unitOfWorkManager.Current.GetTenantId().ShouldBe(1); //Because nested transaction copies outer uow's filters.

                        nestedUow.Complete();
                    }

                    unitOfWorkManager.Current.GetTenantId().ShouldBe(1);
                }

                unitOfWorkManager.Current.GetTenantId().ShouldBe(null);

                outerUow.Complete();
            }
        }
    }
}
