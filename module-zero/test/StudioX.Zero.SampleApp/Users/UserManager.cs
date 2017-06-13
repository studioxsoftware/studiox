using StudioX.Authorization;
using StudioX.Authorization.Users;
using StudioX.Configuration;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.IdentityFramework;
using StudioX.Localization;
using StudioX.Organizations;
using StudioX.Runtime.Caching;
using StudioX.Zero.SampleApp.Roles;

namespace StudioX.Zero.SampleApp.Users
{
    public class UserManager : StudioXUserManager<Role, User>
    {
        public UserManager(
            UserStore userStore, 
            RoleManager roleManager, 
            IPermissionManager permissionManager, 
            IUnitOfWorkManager unitOfWorkManager, 
            ICacheManager cacheManager, 
            IRepository<OrganizationUnit, long> organizationUnitRepository, 
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository, 
            IOrganizationUnitSettings organizationUnitSettings,
            ILocalizationManager localizationManager,
            ISettingManager settingManager,
            IdentityEmailMessageService emailService,
            IUserTokenProviderAccessor userTokenProviderAccessor)
            : base(
                  userStore, 
                  roleManager, 
                  permissionManager, 
                  unitOfWorkManager, 
                  cacheManager, 
                  organizationUnitRepository, 
                  userOrganizationUnitRepository, 
                  organizationUnitSettings,
                  localizationManager,
                  emailService,
                  settingManager,
                  userTokenProviderAccessor)
        {
        }
    }
}