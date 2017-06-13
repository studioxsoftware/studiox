using StudioX.Application.Services;
using StudioX.Domain.Repositories;

namespace StudioX.TestBase.SampleApplication.Messages
{
    public class AsyncMessageAppService : AsyncCrudAppService<Message, MessageDto>, IAsyncMessageAppService
    {
        public AsyncMessageAppService(IRepository<Message> repository)
            : base(repository)
        {

        }
    }
}