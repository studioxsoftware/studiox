using System.Data.Entity;
using System.Linq;
using StudioX.Configuration.Startup;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.TestBase.SampleApplication.Messages;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.ContactLists
{
    public class QueryingTests : SampleApplicationTestBase
    {
        private readonly IRepository<Message> messageRepository;

        public QueryingTests()
        {
            messageRepository = Resolve<IRepository<Message>>();
        }

        protected override void CreateInitialData()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            base.CreateInitialData();
        }

        [Fact]
        public void SimpleQueryingWithAsNoTracking()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var messages = messageRepository
                    .GetAll()
                    .AsNoTracking()
                    .ToList();

                messages.Any().ShouldBeTrue();

                uow.Complete();
            }
        }
    }
}