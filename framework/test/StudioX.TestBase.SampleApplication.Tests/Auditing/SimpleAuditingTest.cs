using System.Threading.Tasks;
using StudioX.Auditing;
using StudioX.TestBase.SampleApplication.People;
using StudioX.TestBase.SampleApplication.People.Dto;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Auditing
{
    public class SimpleAuditingTest : SampleApplicationTestBase
    {
        private readonly IPersonAppService personAppService;

        private IAuditingStore auditingStore;

        public SimpleAuditingTest()
        {
            personAppService = Resolve<IPersonAppService>();
            Resolve<IAuditingConfiguration>().IsEnabledForAnonymousUsers = true;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();
            auditingStore = Substitute.For<IAuditingStore>();
            LocalIocManager.IocContainer.Register(
                Component.For<IAuditingStore>().UsingFactoryMethod(() => auditingStore).LifestyleSingleton()
                );
        }

        #region CASES WRITE AUDIT LOGS

        [Fact]
   
        public async Task ShouldWriteAuditsForConventionalMethods()
        {
            /* All application service methods are audited as conventional. */

            await personAppService.CreatePersonAsync(new CreatePersonInput { ContactListId = 1, Name = "john" });

            #pragma warning disable 4014
            auditingStore.Received().SaveAsync(Arg.Any<AuditInfo>());
            #pragma warning restore 4014
        }

        [Fact]
        public void ShouldWriteAuditsForAuditedClassVirtualMethodsAsDefault()
        {
            Resolve<MyServiceWithClassAudited>().Test1();
            auditingStore.Received().SaveAsync(Arg.Any<AuditInfo>());
        }

        [Fact]
        public void ShouldWriteAuditsForAuditedMethods()
        {
            Resolve<MyServiceWithMethodAudited>().Test1();
            auditingStore.Received().SaveAsync(Arg.Any<AuditInfo>());
        }

        #endregion

        #region CASES DON'T WRITE AUDIT LOGS

        [Fact]
        public void ShouldNotWriteAuditsForConventionalMethodsIfDisabledAuditing()
        {
            /* GetPeople has DisableAuditing attribute. */

            personAppService.GetPeople(new GetPeopleInput());
            auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
        }

        [Fact]
        public void ShouldNotWriteAuditsForAuditedClassNonVirtualMethodsAsDefault()
        {
            Resolve<MyServiceWithClassAudited>().Test2();
            auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
        }

        [Fact]
        public void ShouldNotWriteAuditsForNotAuditedMethods()
        {
            Resolve<MyServiceWithMethodAudited>().Test2();
            auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
        }

        [Fact]
        public void ShouldNotWriteAuditsForNotAuditedClasses()
        {
            Resolve<MyServiceWithNotAudited>().Test1();
            auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
        }


        [Fact]
        public void ShouldNotWriteAuditsIfDisabled()
        {
            Resolve<IAuditingConfiguration>().IsEnabled = false;
            Resolve<MyServiceWithMethodAudited>().Test1();
            auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
        }

        #endregion

        [Audited]
        public class MyServiceWithClassAudited
        {
            public virtual void Test1()
            {

            }

            public void Test2()
            {

            }
        }

        public class MyServiceWithMethodAudited
        {
            [Audited]
            public virtual void Test1()
            {

            }

            public virtual void Test2()
            {

            }
        }

        public class MyServiceWithNotAudited
        {
            public virtual void Test1()
            {

            }
        }
    }
}
