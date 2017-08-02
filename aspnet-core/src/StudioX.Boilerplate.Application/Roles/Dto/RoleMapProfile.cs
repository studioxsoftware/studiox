using AutoMapper;
using StudioX.Authorization;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Dependency;
using StudioX.Domain.Repositories;

namespace StudioX.Boilerplate.Roles.Dto
{
    public class RoleMapProfile : Profile
    {
        public RoleMapProfile()
        {
            // Role and permission
            CreateMap<Permission, string>().ConvertUsing(r => r.Name);
            CreateMap<RolePermissionSetting, string>().ConvertUsing(r => r.Name);

            CreateMap<CreateRoleInput, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());
            CreateMap<UpdateRoleInput, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());

            IRepository<Role, int> repository = IocManager.Instance.Resolve<IRepository<Role, int>>();
            // User and role
            CreateMap<UserRole, string>().ConvertUsing((r) => {
                //TODO: Fix, this seems hacky
                Role role = repository.FirstOrDefault(r.RoleId);
                return role.DisplayName;
            });

            IocManager.Instance.Release(repository);
        }
    }
}