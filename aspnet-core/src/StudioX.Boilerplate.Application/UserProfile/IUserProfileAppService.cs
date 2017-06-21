using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Boilerplate.UserProfile.Dto;

namespace StudioX.Boilerplate.UserProfile
{
    public interface IUserProfileAppService : IApplicationService
    {
        Task<UserProfileInfoDto> Get();

        Task UpdateUserProfile(UpdateUserProfileInput input);

        Task ChangePassword(ChangePasswordInput input);
    }
}