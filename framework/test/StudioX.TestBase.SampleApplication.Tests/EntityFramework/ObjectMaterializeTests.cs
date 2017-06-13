using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.TestBase.SampleApplication.Crm;
using StudioX.Timing;

namespace StudioX.TestBase.SampleApplication.Tests.EntityFramework
{
    public class ObjectMaterializeTests : SampleApplicationTestBase
    {
        private readonly IRepository<Company> companyRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public ObjectMaterializeTests()
        {
            companyRepository = Resolve<IRepository<Company>>();
            unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            if (RandomHelper.GetRandomOf(new[] { 1, 2 }) == 1)
            {
                Clock.Provider = ClockProviders.Local;
            }
            else
            {
                Clock.Provider = ClockProviders.Utc;
            }
        }

        //Note: The code below is cancelled since Effort does not work well with ObjectMaterialized event
        //[Fact] 
        public void DateTimeKindPropertShouldBeNormalizedOnEfObjectMaterialition()
        {
            //using (var uow = unitOfWorkManager.Begin())
            //{
            //    var companies = companyRepository.GetAll().Include(c => c.Branches).ToList();

            //    foreach (var company in companies)
            //    {
            //        company.CreationTime.Kind.ShouldBe(Clock.Kind);


            //        //company.BillingAddress.CreationTime.Kind.ShouldBe(Clock.Kind);
            //        //company.BillingAddress.LastModifier.ModificationTime.Value.Kind.ShouldBe(Clock.Kind);

            //        //company.ShippingAddress.CreationTime.Kind.ShouldBe(Clock.Kind);
            //        //company.ShippingAddress.LastModifier.ModificationTime.Value.Kind.ShouldBe(Clock.Kind);

            //        //company.Branches.ForEach(branch =>
            //        //{
            //        //    branch.CreationTime.Kind.ShouldBe(Clock.Kind);
            //        //});
            //    }

            //    uow.Complete();
            //}
        }
    }
}
