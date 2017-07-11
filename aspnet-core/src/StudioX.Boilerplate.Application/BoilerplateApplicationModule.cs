using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.AutoMapper;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Configuration;
using StudioX.Boilerplate.Roles.Dto;
using StudioX.Boilerplate.Users.Dto;
using StudioX.Domain.Repositories;

namespace StudioX.Boilerplate
{
    [DependsOn(
        typeof(BoilerplateCoreModule), 
        typeof(StudioXAutoMapperModule))]
    public class BoilerplateApplicationModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<BoilerplateAuthorizationProvider>();

            Configuration.Settings.Providers.Add < BoilerplateSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(BoilerplateApplicationModule).GetAssembly());

            // TODO: Is there somewhere else to store these, with the dto classes
            Configuration.Modules.StudioXAutoMapper().Configurators.Add(cfg =>
            {
                // Role and permission
                cfg.CreateMap<Permission, string>().ConvertUsing(r => r.Name);
                cfg.CreateMap<RolePermissionSetting, string>().ConvertUsing(r => r.Name);

                cfg.CreateMap<CreateRoleInput, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());
                cfg.CreateMap<UpdateRoleInput, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());

                IRepository<Role, int> repository = IocManager.Resolve<IRepository<Role, int>>();
               
                // User and role
                cfg.CreateMap<UserRole, string>().ConvertUsing((r) => {
                    //TODO: Fix, this seems hacky
                    Role role = repository.FirstOrDefault(r.RoleId);
                    return role.DisplayName;
                });

                IocManager.Release(repository);

                cfg.CreateMap<UpdateUserInput, User>();
                cfg.CreateMap<UpdateUserInput, User>().ForMember(x => x.Roles, opt => opt.Ignore());

                cfg.CreateMap<CreateUserInput, User>();
                cfg.CreateMap<CreateUserInput, User>().ForMember(x => x.Roles, opt => opt.Ignore());
            });
        }
    }
}