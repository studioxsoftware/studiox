using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Boilerplate.Sessions.Dto;

namespace StudioX.Boilerplate.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
