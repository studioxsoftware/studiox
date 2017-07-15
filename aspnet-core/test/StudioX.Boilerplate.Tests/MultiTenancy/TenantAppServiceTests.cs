using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using StudioX.Boilerplate.EntityFrameworkCore;
using StudioX.Boilerplate.MultiTenancy;
using StudioX.Boilerplate.MultiTenancy.Dto;
using Xunit;

namespace StudioX.Boilerplate.Tests.MultiTenancy
{
    public class TenantAppServiceTests :
        BoilerplateAsyncServiceTestBase<Tenant, TenantDto, int, TenantAppService, CreateTenantInput, UpdateTenantInput>
    {
        public TenantAppServiceTests()
        {
            LoginAsHost("admin");
        }

        protected override async Task<Tenant> CreateEntity(int number)
        {
            return new Tenant
            {
                TenancyName = $"Tenant{number:000.#}",
                Name = $"Tenant{number:000.#}",
                IsActive = true
            };
        }

        protected override CreateTenantInput GetCreateInput()
        {
            return new CreateTenantInput
            {
                TenancyName = "newtenant",
                Name = "New Tenant",
                IsActive = true,
                AdminEmailAddress = "new.tenant.admin@studioxsoftware.com",
                ConnectionString = ""
            };
        }

        protected override UpdateTenantInput GetUpdateInput(int key)
        {
            return new UpdateTenantInput
            {
                Id = key,
                TenancyName = "updatedtenant",
                Name = "Updated Tenant",
                IsActive = true,
                AdminEmailAddress = "new.tenant.admin@studioxsoftware.com",
            };
        }

        public override async Task UpdateChecks(BoilerplateDbContext context, TenantDto updatedObject)
        {
            updatedObject.TenancyName.ShouldBe("updatedtenant");
            updatedObject.Name.ShouldBe("Updated Tenant");
            updatedObject.IsActive.ShouldBeTrue();
        }

        [Fact]
        public virtual async Task CreateShouldCreateAdminUser()
        {
            //Arrange
            var input = GetCreateInput();

            //Act
            var tenantDto = await CheckForValidationErrors(async () =>
                await AppService.Create(input)
            ) as TenantDto;

            // Assert
            await UsingDbContextAsync(async context =>
            {
                var adminRole = await context.Roles.FirstOrDefaultAsync(
                        x => x.NormalizedName == "ADMIN" && x.TenantId == tenantDto.Id);

                adminRole.ShouldNotBeNull();

                var adminUser = await context.Users.Include(x => x.Roles)
                    .FirstOrDefaultAsync(
                        e => e.NormalizedUserName == "ADMIN" && e.EmailAddress == input.AdminEmailAddress
                             && e.TenantId == tenantDto.Id);
                adminUser.ShouldNotBeNull();

                adminUser.Roles.Any(x => x.RoleId == adminRole.Id).ShouldBeTrue();
            });
        }
    }
}