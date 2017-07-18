using StudioX.Application.Services;
using StudioX.Authorization;
using StudioX.Domain.Repositories;

namespace StudioX.TestBase.SampleApplication.Messages
{
    [StudioXAuthorize("AsyncMessageAppService_Permission")]
    public class AsyncMessageAppService : AsyncCrudAppService<Message, MessageDto>, IAsyncMessageAppService
    {
        public AsyncMessageAppService(IRepository<Message> repository)
            : base(repository)
        {

        }
    }
}