using System;
using System.Threading.Tasks;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.Runtime.Security;
using StudioX.UI;
using StudioX.Zero.SampleApp.Users.Dto;
using Microsoft.AspNet.Identity;

namespace StudioX.Zero.SampleApp.Users
{
    public class UserAppService : IUserAppService
    {
        private readonly IRepository<User, long> userRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;
        private readonly UserManager userManager;

        public UserAppService(
            IRepository<User, long> userRepository,
            IUnitOfWorkManager unitOfWorkManager,
            UserManager userManager)
        {
            this.userRepository = userRepository;
            this.unitOfWorkManager = unitOfWorkManager;
            this.userManager = userManager;
        }

        public void CreateUser(CreateUserInput input)
        {
            userRepository.Insert(new User
            {
                TenantId = null,
                UserName = input.UserName,
                FirstName = input.FirstName,
                LastName = input.LastName,
                EmailAddress = input.EmailAddress,
                IsEmailConfirmed = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            });
        }

        public void UpdateUser(UpdateUserInput input)
        {
            var user = userRepository.Get(input.Id);

            user.TenantId = null;
            user.UserName = input.UserName;
            user.FirstName = input.FirstName;
            user.LastName = input.LastName;
            user.EmailAddress = input.EmailAddress;
            user.IsEmailConfirmed = true;
            user.LastLoginTime = input.LastLoginTime;
            user.Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw=="; //123qwe

            userRepository.Update(user);
        }

        public void DeleteUser(long userId)
        {
            userRepository.Delete(userId);
        }

        public virtual async Task ResetPassword(ResetPasswordInput input)
        {
            unitOfWorkManager.Current.SetTenantId(input.TenantId);

            var user = await userManager.GetUserByIdAsync(input.UserId);
            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
            {
                throw new UserFriendlyException("InvalidPasswordResetCode", "InvalidPasswordResetCode_Detail");
            }

            user.Password = new PasswordHasher().HashPassword(input.Password);
            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;

            await userManager.UpdateAsync(user);
        }
    }
}