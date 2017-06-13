using StudioX.Domain.Repositories;
using StudioX.EntityFrameworkCore.Tests.Domain;
using StudioX.Events.Bus;
using Shouldly;
using Xunit;

namespace StudioX.EntityFrameworkCore.Tests.Tests
{
    public class DomainEventsTests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> blogRepository;
        private readonly IEventBus eventBus;

        public DomainEventsTests()
        {
            blogRepository = Resolve<IRepository<Blog>>();
            eventBus = Resolve<IEventBus>();
        }

        [Fact]
        public void ShouldTriggerDomainEventsForAggregateRoot()
        {
            //Arrange

            var isTriggered = false;

            eventBus.Register<BlogUrlChangedEventData>((data) =>
            {
                data.OldUrl.ShouldBe("http://testblog1.myblogs.com");
                isTriggered = true;
            });

            //Act

            var blog1 = blogRepository.Single(b => b.Name == "test-blog-1");
            blog1.ChangeUrl("http://testblog1-changed.myblogs.com");
            blogRepository.Update(blog1);

            //Assert

            isTriggered.ShouldBeTrue();
        }
    }
}