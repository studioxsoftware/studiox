using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Application.Services.Dto;
using StudioX.Authorization;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.Users.Dto;
using StudioX.Configuration;
using StudioX.Boilerplate.Configuration;
using StudioX.Authorization.Users;
using StudioX.AutoMapper;
using StudioX.Collections.Extensions;
using StudioX.Extensions;
using StudioX.Linq.Extensions;
using StudioX.UI;
using StudioX.Boilerplate.BaseModel;
using Microsoft.AspNetCore.Identity;
using System.Linq.Dynamic.Core;
using StudioX.Domain.Repositories;

namespace StudioX.Boilerplate.Users
{
    [StudioXAuthorize(PermissionNames.System.Administration.Users.MainMenu)]
    public class UserAppService :BoilerplateAppServiceBase, IUserAppService
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


        [StudioXAuthorize(PermissionNames.System.Administration.Users.View)]
        public async Task<ListResultDto<UserListDto>> GetAll()
        {
            var users = await userRepository.GetAllListAsync();

            return new ListResultDto<UserListDto>(
                    ObjectMapper.Map<List<UserListDto>>(users)
                );
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Users.View)]
        public PagedResultDto<UserListDto> PagedResult(GetUsersInput input)
        {
            if (input.MaxResultCount <= 0)
                input.MaxResultCount = SettingManager.GetSettingValue<int>(BoilerplateSettingProvider.UsersDefaultPageSize);

            if (input.Sorting.IsNullOrEmpty())
                input.Sorting = InputConstant.DefaultSorting;

            var query = CreateFilteredQuery(input);

            var users = query.OrderBy(input.Sorting)
                .PageBy(input)
                .ToList();

            var totalCount = query.Count();
            return new PagedResultDto<UserListDto>
            {
                TotalCount = totalCount,
                Items = users.MapTo<List<UserListDto>>()
            };
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Users.View)]
        public async Task<UserDto> Get(long id)
        {
            var user = await UserManager.GetUserByIdAsync(id);
            return user.MapTo<UserDto>();
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Users.ChangePermission)]
        public async Task ProhibitPermission(ProhibitPermissionInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            var permission = PermissionManager.GetPermission(input.PermissionName);

            await UserManager.ProhibitPermissionAsync(user, permission);
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Users.ChangePermission)]
        public async Task ResetUserSpecificPermissions(long id)
        {
            var user = await UserManager.GetUserByIdAsync(id);
            await UserManager.ResetAllPermissionsAsync(user);
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Users.Edit)]
        public async Task Unlock(long id)
        {
            var user = await UserManager.GetUserByIdAsync(id);
            await UserManager.ResetAccessFailedCountAsync(user);
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Users.ChangePermission)]
        public async Task UpdateUserPermissions(UserPermissionsInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);

            var grantedPermissions = PermissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissionNames.Contains(p.Name))
                .ToList();

            await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Users.Create)]
        public async Task Create(CreateUserInput input)
        {
            var user = input.MapTo<User>();
            user.TenantId = StudioXSession.TenantId;
            user.Password = passwordHasher.HashPassword(user, input.Password);
            user.IsEmailConfirmed = true;
            user.Roles = new List<UserRole>();

            CheckErrors(await UserManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync();

           // await UserManager.SetRoles(user, input.RoleNames);
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Users.Edit)]
        public async Task Update(UpdateUserInput input)
        {
            var result = await UserManager.CheckDuplicateUsernameOrEmailAddressAsync(input.Id, input.UserName, input.EmailAddress);
            if (!result.Succeeded)
            {
                throw new UserFriendlyException(result.Errors.JoinAsString(" "));
            }

            var user = await UserManager.GetUserByIdAsync(input.Id);
            var mapped = input.MapTo(user);
            mapped.IsEmailConfirmed = true;

            await UserManager.UpdateAsync(mapped);
            await UserManager.SetRoles(user, input.RoleNames);
        }


        [StudioXAuthorize(PermissionNames.System.Administration.Users.Edit)]
        public async Task ResetPassword(ChangeUserPasswordInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var identityResult = await UserManager.ChangePasswordAsync(user, input.Password);

            if (!identityResult.Succeeded)
            {
                throw new UserFriendlyException(identityResult.Errors.JoinAsString(", "));
            }
        }

        [StudioXAuthorize(PermissionNames.System.Administration.Users.Delete)]
        public async Task Delete(long id)
        {
            var user = await UserManager.GetUserByIdAsync(id);
            if (user != null)
            {
                await UserManager.ResetAllPermissionsAsync(user);
                await UserManager.DeleteAsync(user);
            }
        }

        private IQueryable<User> CreateFilteredQuery(GetUsersInput input)
        {
            var query = UserManager.Users;

            if (!input.SearchString.IsNullOrEmpty())
            {
                var searchString = input.SearchString.Trim().ToLower();
                query = query.Where(u => u.UserName.ToLower().Contains(searchString)
                                         || u.FirstName.ToLower().Contains(searchString)
                                         || u.LastName.ToLower().Contains(searchString)
                );
            }

            if (!input.RoleIds.IsNullOrEmpty())
                query = query.Where(u => u.Roles.Any(r => input.RoleIds.Contains(r.RoleId)));

            if (!input.PermissionNames.IsNullOrEmpty())
                query = query.Where(u => u.Permissions.Any(p => input.PermissionNames.Contains(p.Name)));
            return query;
        }
    }
}