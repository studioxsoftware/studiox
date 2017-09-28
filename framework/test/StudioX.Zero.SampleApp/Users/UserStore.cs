using StudioX.Authorization.Users;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Zero.SampleApp.Roles;

namespace StudioX.Zero.SampleApp.Users
{
    public class UserStore : StudioXUserStore<Role, User>
    {
        public UserStore(
            IRepository<User, long> userRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IRepository<UserPermissionSetting, long> userPermissionSettingRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserClaim, long> userClaimRepository)
            : base(
                userRepository,
                userLoginRepository,
                userRoleRepository,
                roleRepository,
                userPermissionSettingRepository,
                unitOfWorkManager,
                userClaimRepository)
        {

        }
    }
}