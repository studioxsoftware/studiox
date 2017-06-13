using System;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Application.Services.Dto;
using StudioX.TestBase.SampleApplication.Messages;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Messages
{
    public class MessageAppServiceTests : SampleApplicationTestBase
    {
        private readonly MessageAppService messageAppService;
        private readonly IAsyncMessageAppService asyncMessageAppService;

        public MessageAppServiceTests()
        {
            messageAppService = Resolve<MessageAppService>();
            asyncMessageAppService = Resolve<IAsyncMessageAppService>();
        }

        protected override void CreateInitialData()
        {
            UsingDbContext(
                context =>
                {
                    context.Messages.Add(
                        new Message
                        {
                            TenantId = 1,
                            Text = "tenant-1-message-1"
                        });

                    context.Messages.Add(
                        new Message
                        {
                            TenantId = 1,
                            Text = "tenant-1-message-2"
                        });
                });
        }

        [Fact]
        public void ShouldGetAllMessages()
        {
            //Act

            var messages = messageAppService.GetAll(new GetMessagesWithFilterInput());

            //Assert

            messages.TotalCount.ShouldBe(2);
            messages.Items.Count.ShouldBe(2);
        }

        [Fact]
        public void ShouldGetAllMessagesWithFiltering()
        {
            //Act

            var messages = messageAppService.GetAll(new GetMessagesWithFilterInput { Text = "message-1" });

            //Assert

            messages.TotalCount.ShouldBe(1);
            messages.Items.Count.ShouldBe(1);
            messages.Items[0].Text.ShouldBe("tenant-1-message-1");
        }

        [Fact]
        public void ShouldGetAllMessagesWithPaging()
        {
            //Act

            var messages = messageAppService.GetAll(new GetMessagesWithFilterInput { MaxResultCount = 1 });

            //Assert

            messages.TotalCount.ShouldBe(2);
            messages.Items.Count.ShouldBe(1);
            messages.Items[0].Text.ShouldBe("tenant-1-message-2");
        }

        [Fact]
        public void ShouldGetAllMessagesWithPagingAndSorting()
        {
            //Act

            var messages = messageAppService.GetAll(new GetMessagesWithFilterInput { MaxResultCount = 1, Sorting = "Text" });

            //Assert

            messages.TotalCount.ShouldBe(2);
            messages.Items.Count.ShouldBe(1);
            messages.Items[0].Text.ShouldBe("tenant-1-message-1");
        }

        [Fact]
        public async Task ShouldGetAllMessagesWithFilteringAsync()
        {
            //Act

            var messages = await asyncMessageAppService.GetAll(new PagedAndSortedResultRequestDto());

            //Assert

            messages.TotalCount.ShouldBe(2);
            messages.Items.Count.ShouldBe(2);
        }

        [Fact]
        public void ShouldGetMessage()
        {
            //Act

            var message = messageAppService.Get(new EntityDto(2));

            //Assert

            message.Text.ShouldBe("tenant-1-message-2");
        }

        [Fact]
        public void ShouldDeleteMessage()
        {
            //Arrange

            UsingDbContext(context =>
            {
                context.Messages.FirstOrDefault(m => m.Text == "tenant-1-message-2").ShouldNotBeNull();
            });

            //Act

            messageAppService.Delete(new EntityDto(2));

            //Assert

            UsingDbContext(context =>
            {
                context.Messages.Single(m => m.Text == "tenant-1-message-2").IsDeleted.ShouldBeTrue();
            });
        }

        [Fact]
        public void ShouldUpdateMessage()
        {
            //Arrange

            UsingDbContext(context =>
            {
                context.Messages.Single(m => m.Id == 2).Text.ShouldBe("tenant-1-message-2");
            });

            //Act

            var message = messageAppService.Get(new EntityDto(2));
            message.Text = "tenant-1-message-2-updated";
            var updatedMessage = messageAppService.Update(message);

            //Assert

            updatedMessage.Text.ShouldBe("tenant-1-message-2-updated");

            UsingDbContext(context =>
            {
                context.Messages.Single(m => m.Id == 2).Text.ShouldBe("tenant-1-message-2-updated");
            });
        }


        [Fact]
        public void ShouldCreateMessage()
        {
            //Arrange

            var messageText = Guid.NewGuid().ToString("N");

            //Act

            var createdMessage = messageAppService.Create(new MessageDto { Text = messageText });

            //Assert

            createdMessage.Id.ShouldBeGreaterThan(0);
            createdMessage.Text.ShouldBe(messageText);

            UsingDbContext(context =>
            {
                context.Messages.FirstOrDefault(m => m.Text == messageText).ShouldNotBeNull();
            });
        }
    }
}
