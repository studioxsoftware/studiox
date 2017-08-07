using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.EntityFrameworkCore;
using StudioX.Boilerplate.Users;
using StudioX.Boilerplate.Users.Dto;
using Xunit;

namespace StudioX.Boilerplate.Tests.Users
{
    public class UserAppServiceTests :
        BoilerplateAsyncServiceTestBase<User, UserDto, long, UserAppService, CreateUserInput, UpdateUserInput>
    {
        protected override async Task<User> CreateEntity(int entityNumer)
        {
            return new User
            {
                EmailAddress = $"user.{entityNumer:000.#}@studioxsoftware.com",
                IsActive = true,
                FirstName = "User",
                LastName = $"{entityNumer:000.#}",
                Password = "123qwe",
                UserName = $"User.{entityNumer:000.#}",
                TenantId = StudioXSession.TenantId
            };
        }

        protected override CreateUserInput GetCreateInput()
        {
            return new CreateUserInput
            {
                EmailAddress = "new.user@studioxsoftware.com",
                IsActive = true,
                FirstName = "New",
                LastName = "User",
                Password = "123qwe",
                ConfirmPassword = "123qwe",
                UserName = "New.User",
                RoleNames = new[] {"Admin"}
            };
        }

        protected override UpdateUserInput GetUpdateInput(long key)
        {
            return new UpdateUserInput
            {
                Id = key,
                EmailAddress = "updated.user@studioxsoftware.com",
                IsActive = true,
                FirstName = "Updated",
                LastName = "User",
                UserName = "Updated.User",
                RoleNames = new[] {"Admin"}
            };
        }

        private async Task<Role> GetRole(string name)
        {
            return await UsingDbContextAsync(
                    async context =>
                    {
                        return await context.Roles.FirstOrDefaultAsync(
                                r => r.Name == name && r.TenantId == StudioXSession.TenantId && r.IsDeleted == false);
                    });
        }

        public override async Task CreateChecks(BoilerplateDbContext context, CreateUserInput createEntity)
        {
            var adminRole = await GetRole("Admin");
            var user =
                await context.Users.Include(x => x.Roles)
                    .FirstOrDefaultAsync(e => e.EmailAddress == createEntity.EmailAddress);

            adminRole.ShouldNotBeNull();
            user.ShouldNotBeNull();

            user.Roles.Any(x => x.RoleId == adminRole.Id);
        }

        [Fact]
        public async Task CreateForNonExistingRoleShouldThrowExceptionTest()
        {
            //Arrange
            var createInput = GetCreateInput();
            createInput.RoleNames = new[] {"NONEXISTANTROLE"};

            //Act, Assert
            await AppService.Create(createInput).ShouldThrowAsync(typeof(StudioXException));
        }

        // This is not a Data Attribute test
        [Fact]
        public async Task CreateWithInvalidUserNameShouldThrow()
        {
            //Arrange
            var createInput = GetCreateInput();
            createInput.UserName = "invalid username";

            //Act, Assert
            await CheckForValidationErrors(
                async () => await AppService.Create(createInput)
            ).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task GetAllCheckSortingTest()
        {
            //Arrange
            await Create(20);

            //Act
            var users = await AppService.GetAll(
                new PagedResultRequestDto {MaxResultCount = 10, SkipCount = 10}
            );

            //Assert
            users.Items.ShouldBeInOrder(SortDirection.Descending,
                Comparer<UserDto>.Create((x, y) => x.FirstName.CompareTo(y.FirstName))
            );
        }

        [Fact]
        public async Task UpdateSetsRolesCorrectlyTest()
        {
            //Arrange
            var adminRole = await GetRole("admin");

            await Create(1); //User Has Admin Permission

            var user = GetUpdateInput(keys[0]);
            user.RoleNames = new List<string>().ToArray(); //Remove Admin Role

            // Act
            var userDto = await CheckForValidationErrors(
                async () => await AppService.Update(
                    user
                )
            );

            // Assert
            await UsingDbContextAsync(async context =>
            {
                var updatedUser = await context.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == userDto.Id);
                updatedUser.Id.ShouldBe(keys[0]);

                // Admin role should be removed
                updatedUser.Roles.Any(x => x.Id == adminRole.Id).ShouldBeFalse();
            });
        }

        public override async Task DeleteChecks(BoilerplateDbContext context, long key)
        {
            var user = await context.Users.FirstOrDefaultAsync(
                e => e.Id == key
            );

            user.ShouldNotBeNull();
            user.IsDeleted.ShouldBeTrue();
        }
    }
}