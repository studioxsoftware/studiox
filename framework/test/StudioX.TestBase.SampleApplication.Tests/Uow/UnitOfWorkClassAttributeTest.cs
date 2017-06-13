using System.Linq;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Uow
{
    public class UnitOfWorkClassAttributeTest : SampleApplicationTestBase
    {
        private readonly MyUowMarkedClass uowClass;
        private readonly MyDisabledUowMarkedClass disabledUowClass;

        public UnitOfWorkClassAttributeTest()
        {
            uowClass = Resolve<MyUowMarkedClass>();
            disabledUowClass = Resolve<MyDisabledUowMarkedClass>();
        }

        [Fact]
        public void ShouldInterceptUowMarkedClasses()
        {
            uowClass.GetPeopleCount().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void ShouldNotInterceptNonUowMarkedMethods()
        {
            uowClass.NonUowMethod();
        }

        [Fact]
        public void ShouldInterceptUowMarkedMethodsOnDisabledUowClasses()
        {
            disabledUowClass.GetPeopleCount().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void ShouldNotInterceptNonUowMarkedClasses()
        {
            disabledUowClass.NonUowMethod();
        }
    }

    [UnitOfWork]
    public class MyUowMarkedClass : ITransientDependency
    {
        private readonly IRepository<Person> personRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public MyUowMarkedClass(IRepository<Person> personRepository, IUnitOfWorkManager unitOfWorkManager)
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

    [UnitOfWork(IsDisabled = true)]
    public class MyDisabledUowMarkedClass : ITransientDependency
    {
        private readonly IRepository<Person> personRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public MyDisabledUowMarkedClass(IRepository<Person> personRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            this.personRepository = personRepository;
            this.unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork(IsDisabled = false)]
        public virtual int GetPeopleCount()
        {
            //GetAll can be only used inside a UOW. This should work since MyCustomUowClass is UOW by custom convention.
            unitOfWorkManager.Current.ShouldNotBeNull();
            return personRepository.GetAll().Count();
        }

        public virtual void NonUowMethod()
        {
            unitOfWorkManager.Current.ShouldBeNull();
        }
    }
}
