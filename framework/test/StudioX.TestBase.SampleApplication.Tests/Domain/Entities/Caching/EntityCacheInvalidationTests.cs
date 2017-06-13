using StudioX.AutoMapper;
using StudioX.Dependency;
using StudioX.Domain.Entities.Caching;
using StudioX.Domain.Repositories;
using StudioX.Runtime.Caching;
using StudioX.TestBase.SampleApplication.Messages;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Domain.Entities.Caching
{
    public class EntityCacheInvalidationTests : SampleApplicationTestBase
    {
        private readonly IMessageCache messageCache;
        private readonly IRepository<Message> messageRepository;

        public EntityCacheInvalidationTests()
        {
            messageCache = Resolve<IMessageCache>();
            messageRepository = Resolve<IRepository<Message>>();
        }

        [Fact]
        public void CachedEntitiesShouldBeRefreshedOnChange()
        {
            //Arrange
            StudioXSession.TenantId = 1;
            var message1 = messageRepository.Single(m => m.Text == "tenant-1-message-1");

            //Act & Assert
            messageCache.Get(message1.Id).Text.ShouldBe(message1.Text);

            //Arrange: Update the entity
            message1.Text = "host-message-1-updated";
            messageRepository.Update(message1);

            //Act & Assert: Cached object should be updated
            messageCache.Get(message1.Id).Text.ShouldBe(message1.Text);
        }

        public interface IMessageCache : IEntityCache<MessageCacheItem>
        {

        }

        public class MessageCache : EntityCache<Message, MessageCacheItem>, IMessageCache, ITransientDependency
        {
            public MessageCache(ICacheManager cacheManager, IRepository<Message, int> repository)
                : base(cacheManager, repository)
            {

            }
        }

        [AutoMapFrom(typeof(Message))]
        public class MessageCacheItem
        {
            public string Text { get; set; }
        }
    }
}
