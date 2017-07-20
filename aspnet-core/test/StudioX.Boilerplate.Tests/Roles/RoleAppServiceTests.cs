using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.EntityFrameworkCore;
using StudioX.Boilerplate.Roles;
using StudioX.Boilerplate.Roles.Dto;
using Xunit;

namespace StudioX.Boilerplate.Tests.Roles
{
    public class RoleAppServiceTests :
        BoilerplateAsyncServiceTestBase<Role, RoleDto, int, RoleAppService, CreateRoleInput, UpdateRoleInput>
    {
        protected override async Task<Role> CreateEntity(int number)
        {
            return new Role
            {
                DisplayName = $"Role{number:000.#}",
                Description = $"Role{number:000.#}",
                Name = $"Role{number:000.#}",
                TenantId = StudioXSession.TenantId
            };
        }

        protected override CreateRoleInput GetCreateInput()
        {
            return new CreateRoleInput
            {
                Name = "New Role",
                DisplayName = "New Role",
                Description = "New Role Description",
                Permissions = new List<string>
                {
                    PermissionNames.System.Administration.Users.MainMenu,
                    PermissionNames.System.Administration.Roles.MainMenu
                }
            };
        }

        protected override UpdateRoleInput GetUpdateInput(int key)
        {
            return new UpdateRoleInput
            {
                Id = key,
                Name = "Updated Role",
                DisplayName = "Updated Role",
                Description = "Updated Role Description",
                Permissions = new List<string>()
            };
        }

        public override async Task UpdateChecks(BoilerplateDbContext context, RoleDto updateDto)
        {
            updateDto.Name.ShouldBe("Updated Role");
            updateDto.DisplayName.ShouldBe("Updated Role");
            updateDto.Description.ShouldBe("Updated Role Description");

            var role = await context.Roles.FirstOrDefaultAsync(x => x.Id == updateDto.Id);
            role.ShouldNotBeNull();
        }

        [Fact]
        public virtual async Task CreateSetsCorrectPermissionsTest()
        {
            //Arrange
            var createDto = GetCreateInput();

            //Act
            var createdEntityDto = await AppService.Create(createDto);

            //Assert
            await UsingDbContextAsync(async context =>
            {
                var savedEntity =
                    await context.Roles.Include(x => x.Permissions)
                        .FirstOrDefaultAsync(e => e.Id == createdEntityDto.Id);
                savedEntity.ShouldNotBeNull();

                savedEntity.Permissions.Any(x => x.Name == PermissionNames.System.Administration.Users.MainMenu)
                    .ShouldBeTrue();
                savedEntity.Permissions.Any(x => x.Name == PermissionNames.System.Administration.Roles.MainMenu)
                    .ShouldBeTrue();
            });
        }

        //[Fact]
        //public virtual async Task GetAllSortingTest()
        //{
        //    //Arrange
        //    await Create(20);

        //    //Act
        //    var roles = await AppService.GetAll(
        //        new PagedResultRequestDto {MaxResultCount = 10, SkipCount = 10}
        //    );

        //    //Assert
        //    roles.Items.ShouldBeInOrder(SortDirection.Ascending,
        //        Comparer<RoleDto>.Create((x, y) => x.Name.CompareTo(y.Name))
        //    );

        //    // admin is first role
        //    roles.Items[0].Name.ShouldBe("Role009");
        //    roles.Items[9].Name.ShouldBe("Role018");
        //}
    }
}