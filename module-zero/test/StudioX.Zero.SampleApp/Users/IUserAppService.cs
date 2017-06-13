using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Zero.SampleApp.Users.Dto;

namespace StudioX.Zero.SampleApp.Users
{
    public interface IUserAppService : IApplicationService
    {
        void CreateUser(CreateUserInput input);

        void UpdateUser(UpdateUserInput input);

        void DeleteUser(long userId);

        Task ResetPassword(ResetPasswordInput input);
    }
}
