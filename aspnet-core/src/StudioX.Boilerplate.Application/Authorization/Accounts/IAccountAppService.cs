using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Boilerplate.Authorization.Accounts.Dto;

namespace StudioX.Boilerplate.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
