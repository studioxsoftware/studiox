using System;
using StudioX.Domain.Repositories;
using StudioX.EntityFrameworkCore.Tests.Domain;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Entities;
using Shouldly;
using Xunit;

namespace StudioX.EntityFrameworkCore.Tests.Tests
{
    public class EntityChangeEventsTests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> blogRepository;
        private readonly IEventBus eventBus;

        public EntityChangeEventsTests()
        {
            blogRepository = Resolve<IRepository<Blog>>();
            eventBus = Resolve<IEventBus>();
        }

        [Fact]
        public void ComplexEventTest()
        {
            var blogName = Guid.NewGuid().ToString("N");

            var creatingEventTriggered = false;
            var createdEventTriggered = false;
            var updatingEventTriggered = false;
            var updatedEventTriggered = false;
            var blogUrlChangedEventTriggered = false;

            eventBus.Register<EntityCreatingEventData<Blog>>(data =>
            {
                creatingEventTriggered.ShouldBeFalse();
                createdEventTriggered.ShouldBeFalse();
                updatingEventTriggered.ShouldBeFalse();
                updatedEventTriggered.ShouldBeFalse();
                blogUrlChangedEventTriggered.ShouldBeFalse();

                creatingEventTriggered = true;

                data.Entity.IsTransient().ShouldNotBeNull();
                data.Entity.Name.ShouldBe(blogName);

                /* Want to change url from http:// to https:// (ensure to save https url always)
                 * Expect to trigger EntityUpdatingEventData, EntityUpdatedEventData and BlogUrlChangedEventData events */
                data.Entity.Url.ShouldStartWith("http://");
                data.Entity.ChangeUrl(data.Entity.Url.Replace("http://", "https://"));
            });

            eventBus.Register<EntityCreatedEventData<Blog>>(data =>
            {
                creatingEventTriggered.ShouldBeTrue();
                createdEventTriggered.ShouldBeFalse();
                updatingEventTriggered.ShouldBeTrue();
                updatedEventTriggered.ShouldBeFalse();
                blogUrlChangedEventTriggered.ShouldBeTrue();

                createdEventTriggered = true;

                data.Entity.IsTransient().ShouldNotBeNull();
                data.Entity.Name.ShouldBe(blogName);
            });

            eventBus.Register<EntityUpdatingEventData<Blog>>(data =>
            {
                creatingEventTriggered.ShouldBeTrue();
                createdEventTriggered.ShouldBeFalse();
                updatingEventTriggered.ShouldBeFalse();
                updatedEventTriggered.ShouldBeFalse();
                blogUrlChangedEventTriggered.ShouldBeFalse();

                updatingEventTriggered = true;

                data.Entity.IsTransient().ShouldNotBeNull();
                data.Entity.Name.ShouldBe(blogName);
                data.Entity.Url.ShouldStartWith("https://");
            });

            eventBus.Register<EntityUpdatedEventData<Blog>>(data =>
            {
                creatingEventTriggered.ShouldBeTrue();
                createdEventTriggered.ShouldBeTrue();
                updatingEventTriggered.ShouldBeTrue();
                updatedEventTriggered.ShouldBeFalse();
                blogUrlChangedEventTriggered.ShouldBeTrue();

                updatedEventTriggered = true;

                data.Entity.IsTransient().ShouldNotBeNull();
                data.Entity.Name.ShouldBe(blogName);
                data.Entity.Url.ShouldStartWith("https://");
            });

            eventBus.Register<BlogUrlChangedEventData>(data =>
            {
                creatingEventTriggered.ShouldBeTrue();
                createdEventTriggered.ShouldBeFalse();
                updatingEventTriggered.ShouldBeTrue();
                updatedEventTriggered.ShouldBeFalse();
                blogUrlChangedEventTriggered.ShouldBeFalse();

                blogUrlChangedEventTriggered = true;

                data.Blog.IsTransient().ShouldNotBeNull();
                data.Blog.Name.ShouldBe(blogName);
                data.Blog.Url.ShouldStartWith("https://");
            });

            blogRepository.Insert(new Blog(blogName, "http://studiox.com"));
        }
    }
}
