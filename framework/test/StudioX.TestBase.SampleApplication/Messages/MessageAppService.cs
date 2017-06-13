using System.Linq;
using StudioX.Application.Services;
using StudioX.Domain.Repositories;
using StudioX.Extensions;
using StudioX.Linq.Extensions;

namespace StudioX.TestBase.SampleApplication.Messages
{
    public class MessageAppService : CrudAppService<Message, MessageDto, int, GetMessagesWithFilterInput>
    {
        public MessageAppService(IRepository<Message, int> repository)
            : base(repository)
        {

        }

        protected override IQueryable<Message> CreateFilteredQuery(GetMessagesWithFilterInput input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(!input.Text.IsNullOrWhiteSpace(), m => m.Text.Contains(input.Text));
        }
    }
}