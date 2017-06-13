using StudioX.Authorization;
using StudioX.Authorization.Users;
using StudioX.Configuration;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Zero.Configuration;
using StudioX.Zero.SampleApp.MultiTenancy;
using StudioX.Zero.SampleApp.Roles;
using StudioX.Zero.SampleApp.Users;

namespace StudioX.Zero.SampleApp.Authorization
{
    public class AppLogInManager : StudioXLogInManager<Tenant, Role, User>
    {
        public AppLogInManager(
            UserManager userManager, 
            IMultiTenancyConfig multiTenancyConfig, 
            IRepository<Tenant> tenantRepository, 
            IUnitOfWorkManager unitOfWorkManager, 
            ISettingManager settingManager, 
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository, 
            IUserManagementConfig userManagementConfig, IIocResolver iocResolver, 
            RoleManager roleManager) 
            : base(
                  userManager,
                  multiTenancyConfig, 
                  tenantRepository, 
                  unitOfWorkManager, 
                  settingManager, 
                  userLoginAttemptRepository, 
                  userManagementConfig, 
                  iocResolver, 
                  roleManager)
        {
        }
    }
}
