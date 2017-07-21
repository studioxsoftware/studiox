using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StudioX.Authorization;
using StudioX.AutoMapper;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.UserProfile.Dto;
using StudioX.Collections.Extensions;
using StudioX.Runtime.Session;
using StudioX.UI;

namespace StudioX.Boilerplate.UserProfile
{
    [StudioXAuthorize]
    public class UserProfileAppService : BoilerplateAppServiceBase, IUserProfileAppService
    {
        private readonly IPasswordHasher<User> passwordHasher;

        public UserProfileAppService(IPasswordHasher<User> passwordHasher)
        {
            this.passwordHasher = passwordHasher;
        }

        public async Task<UserProfileInfoDto> Get()
        {
            return (await GetCurrentUserAsync()).MapTo<UserProfileInfoDto>();
        }

        public async Task UpdateUserProfile(UpdateUserProfileInput input)
        {
            var user = await UserManager.GetUserByIdAsync(StudioXSession.GetUserId());
            var mapped = input.MapTo(user);
            await UserManager.UpdateAsync(mapped);
        }

        public async Task ChangePassword(ChangePasswordInput input)
        {
            var user = await UserManager.GetUserByIdAsync(StudioXSession.GetUserId());
            var hashedPassword = passwordHasher.HashPassword(user, input.Password);
            var verificationResult = passwordHasher.VerifyHashedPassword(user, hashedPassword, input.Password);

            if (verificationResult != PasswordVerificationResult.Success)
            {
                throw new UserFriendlyException("Current password must match old password!");
            }

            var identityResult = await UserManager.ChangePasswordAsync(user, input.NewPassword);
            if (!identityResult.Succeeded)
            {
                throw new UserFriendlyException(identityResult.Errors.JoinAsString(", "));
            }
        }
    }
}