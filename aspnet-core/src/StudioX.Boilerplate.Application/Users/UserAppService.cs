using System.Collections.Generic;
using System.Threading.Tasks;
using StudioX.Application.Services.Dto;
using StudioX.Authorization;
using StudioX.Domain.Repositories;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Users.Dto;
using Microsoft.AspNetCore.Identity;

namespace StudioX.Boilerplate.Users
{
    [StudioXAuthorize(PermissionNames.System.Administration.Users.MainMenu)]
    public class UserAppService : BoilerplateAppServiceBase, IUserAppService
    {
        private readonly IRepository<User, long> userRepository;
        private readonly IPasswordHasher<User> passwordHasher;

        public UserAppService(
            IRepository<User, long> userRepository, 
            IPasswordHasher<User> passwordHasher)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }

        public async Task ProhibitPermission(ProhibitPermissionInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            var permission = PermissionManager.GetPermission(input.PermissionName);

            await UserManager.ProhibitPermissionAsync(user, permission);
        }

        //Example for primitive method parameters.
        public async Task RemoveFromRole(long userId, string roleName)
        {
            var user = await UserManager.FindByIdAsync(userId.ToString());
            CheckErrors(await UserManager.RemoveFromRoleAsync(user, roleName));
        }

        public async Task<ListResultDto<UserListDto>> GetUsers()
        {
            var users = await userRepository.GetAllListAsync();

            return new ListResultDto<UserListDto>(
                    ObjectMapper.Map<List<UserListDto>>(users)
                );
        }

        public async Task CreateUser(CreateUserInput input)
        {
            var user = ObjectMapper.Map<User>(input);

            user.TenantId = StudioXSession.TenantId;
            user.Password = passwordHasher.HashPassword(user, input.Password);
            user.IsEmailConfirmed = true;

            CheckErrors(await UserManager.CreateAsync(user));
        }
    }
}