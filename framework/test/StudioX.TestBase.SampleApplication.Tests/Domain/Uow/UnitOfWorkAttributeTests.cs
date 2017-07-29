using Shouldly;
using StudioX.Domain.Services;
using StudioX.Domain.Uow;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Domain.Uow
{
    public class UnitOfWorkAttributeTests : SampleApplicationTestBase
    {
        [Fact]
        public void UnitOfWorkAttributeShouldWorkWithoutVirtualWhenResolvedByInterface()
        {
            var service = Resolve<IMyTestDomainService>();
            service.DoIt();
        }
    }

    public interface IMyTestDomainService : IDomainService
    {
        void DoIt();
    }

    public class MyTestDomainService : DomainService, IMyTestDomainService
    {
        [UnitOfWork]
        public void DoIt()
        {
            CurrentUnitOfWork.ShouldNotBeNull();
        }
    }
}