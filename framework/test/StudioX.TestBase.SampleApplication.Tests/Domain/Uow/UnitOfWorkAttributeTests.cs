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
            service.DoIt<string>();
        }

        [Fact]
        public void UnitOfWorkAttributeShouldWorkWithVirtualWhenResolvedByClass()
        {
            var service = Resolve<MyTestDomainService>();
            service.DoIt2<string>();
        }
    }

    public interface IMyTestDomainService : IDomainService
    {
        void DoIt<T>();
    }

    public class MyTestDomainService : DomainService, IMyTestDomainService
    {
        [UnitOfWork]
        public void DoIt<T>()
        {
            CurrentUnitOfWork.ShouldNotBeNull();
        }

        [UnitOfWork]
        public virtual void DoIt2<T>()
        {
            CurrentUnitOfWork.ShouldNotBeNull();
        }
    }
}