using System.Threading.Tasks;
using StudioX.Application.Features;
using StudioX.Authorization;
using StudioX.Extensions;
using StudioX.TestBase.SampleApplication.ContacLists;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Features
{
    public class FeatureSystemTests: SampleApplicationTestBase
    {
        private readonly IFeatureManager featureManager;

        public FeatureSystemTests()
        {
            featureManager = Resolve<IFeatureManager>();
        }

        [Fact]
        public void ShouldGetDefinedFeatures()
        {
            featureManager.Get(SampleFeatureProvider.Names.Contacts).ShouldNotBe(null);
            featureManager.Get(SampleFeatureProvider.Names.MaxContactCount).ShouldNotBe(null);
            featureManager.GetAll().Count.ShouldBe(2);
        }

        [Fact]
        public void ShouldNotGetUndefinedFeatures()
        {
            featureManager.GetOrNull("NonExistingFeature").ShouldBe(null);
            Assert.Throws<StudioXException>(() => featureManager.Get("NonExistingFeature"));
        }

        [Fact]
        public virtual void ShouldGetFeatureValues()
        {
            var featureValueStore = Substitute.For<IFeatureValueStore>();
            featureValueStore.GetValueOrNullAsync(1, featureManager.Get(SampleFeatureProvider.Names.Contacts)).Returns(Task.FromResult("true"));
            featureValueStore.GetValueOrNullAsync(1, featureManager.Get(SampleFeatureProvider.Names.MaxContactCount)).Returns(Task.FromResult("20"));

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureValueStore>().UsingFactoryMethod(() => featureValueStore).LifestyleSingleton()
                );

            var featureChecker = Resolve<IFeatureChecker>();
            featureChecker.GetValue(SampleFeatureProvider.Names.Contacts).To<bool>().ShouldBeTrue();
            featureChecker.IsEnabled(SampleFeatureProvider.Names.Contacts).ShouldBeTrue();
            featureChecker.GetValue(SampleFeatureProvider.Names.MaxContactCount).To<int>().ShouldBe(20);
        }

        [Fact]
        public void ShouldCallMethodWithFeatureIfEnabled()
        {
            var featureValueStore = Substitute.For<IFeatureValueStore>();
            featureValueStore.GetValueOrNullAsync(1, featureManager.Get(SampleFeatureProvider.Names.Contacts)).Returns(Task.FromResult("true"));

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureValueStore>().UsingFactoryMethod(() => featureValueStore).LifestyleSingleton()
                );

            var contactListAppService = Resolve<IContactListAppService>();
            contactListAppService.Test(); //Should not throw exception
        }

        [Fact]
        public void ShouldNotCallMethodWithFeatureIfNotEnabled()
        {
            var featureValueStore = Substitute.For<IFeatureValueStore>();
            featureValueStore.GetValueOrNullAsync(1, featureManager.Get(SampleFeatureProvider.Names.Contacts)).Returns(Task.FromResult("false"));
            featureValueStore.GetValueOrNullAsync(1, featureManager.Get(SampleFeatureProvider.Names.MaxContactCount)).Returns(Task.FromResult("20"));

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureValueStore>().UsingFactoryMethod(() => featureValueStore).LifestyleSingleton()
                );

            var contactListAppService = Resolve<IContactListAppService>();
            Assert.Throws<StudioXAuthorizationException>(() => contactListAppService.Test());
        }
    }
}
