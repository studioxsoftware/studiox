using System.Linq;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Uow
{
    public class CustomUowConventionTests : SampleApplicationTestBase
    {
        private readonly MyCustomUowClass customUowClass;

        public CustomUowConventionTests()
        {
            customUowClass = Resolve<MyCustomUowClass>();
        }

        [Fact]
        public void ShouldApplyCustomUnitOfWorkConvention()
        {
            customUowClass.GetPeopleCount().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void ShouldNotInterceptNonUowMarkedMethods()
        {
            customUowClass.NonUowMethod();
        }
    }

    public class MyCustomUowClass : ITransientDependency
    {
        private readonly IRepository<Person> personRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public MyCustomUowClass(IRepository<Person> personRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            this.personRepository = personRepository;
            this.unitOfWorkManager = unitOfWorkManager;
        }

        public virtual int GetPeopleCount()
        {
            //GetAll can be only used inside a UOW. This should work since MyCustomUowClass is UOW by custom convention.
            unitOfWorkManager.Current.ShouldNotBeNull();
            return personRepository.GetAll().Count();
        }

        [UnitOfWork(IsDisabled = true)]
        public virtual void NonUowMethod()
        {
            unitOfWorkManager.Current.ShouldBeNull();
        }
    }
}
