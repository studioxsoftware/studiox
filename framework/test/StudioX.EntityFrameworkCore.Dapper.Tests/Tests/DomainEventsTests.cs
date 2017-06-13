using System.Threading.Tasks;

using StudioX.Dapper.Repositories;
using StudioX.Domain.Repositories;
using StudioX.EntityFrameworkCore.Dapper.Tests.Domain;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Entities;

using Shouldly;

using Xunit;

namespace StudioX.EntityFrameworkCore.Dapper.Tests.Tests
{
    public class DomainEventsTests : StudioXEfCoreDapperTestApplicationBase
    {
        private readonly IDapperRepository<Blog> blogDapperRepository;
        private readonly IRepository<Blog> blogRepository;
        private readonly IEventBus eventBus;

        public DomainEventsTests()
        {
            blogRepository = Resolve<IRepository<Blog>>();
            blogDapperRepository = Resolve<IDapperRepository<Blog>>();
            eventBus = Resolve<IEventBus>();
        }

        [Fact]
        public void ShouldTriggerDomainEventsForAggregateRoot()
        {
            //Arrange

            var isTriggered = false;

            eventBus.Register<BlogUrlChangedEventData>(data =>
            {
                data.OldUrl.ShouldBe("http://testblog1.myblogs.com");
                isTriggered = true;
            });

            //Act

            Blog blog1 = blogRepository.Single(b => b.Name == "test-blog-1");
            blog1.ChangeUrl("http://testblog1-changed.myblogs.com");
            blogRepository.Update(blog1);
            

            //Assert
            blogDapperRepository.Get(blog1.Id).ShouldNotBeNull();
            isTriggered.ShouldBeTrue();
        }

        [Fact]
        public async Task shouldtriggereventoninsertedwithdapper()
        {
            var triggerCount = 0;

            Resolve<IEventBus>().Register<EntityCreatedEventData<Blog>>(
                eventData =>
                {
                    eventData.Entity.Name.ShouldBe("OnSoftware");
                    eventData.Entity.IsTransient().ShouldBe(false);
                    triggerCount++;
                });

            blogDapperRepository.Insert(new Blog("OnSoftware", "www.studiox.com"));

            triggerCount.ShouldBe(1);
        }
    }
}
