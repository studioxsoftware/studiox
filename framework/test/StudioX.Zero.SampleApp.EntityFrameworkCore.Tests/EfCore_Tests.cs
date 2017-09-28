using System.Linq;
using System.Threading.Tasks;
using StudioX.Domain.Uow;
using StudioX.Zero.SampleApp.Roles;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.EntityFrameworkCore.Tests
{
    public class EfCoreTests : SimpleTaskAppTestBase
    {
        private readonly RoleManager roleManager;

        public EfCoreTests()
        {
            roleManager = Resolve<RoleManager>();
        }

        [Fact]
        public async Task SeedDataTest()
        {
            await UsingDbContextAsync(async context =>
            {
                (await context.Tenants.CountAsync()).ShouldBe(1);
            });
        }

        [Fact]
        public async Task ShouldCreateAndRetrieveRole()
        {
            await CreateRoleAsync("Role1");

            var role1Retrieved = await roleManager.FindByNameAsync("Role1");
            role1Retrieved.ShouldNotBe(null);
            role1Retrieved.Name.ShouldBe("Role1");
        }

        [Fact]
        public async Task MultiTenancyTestsUsingSession()
        {
            //Switch to host
            StudioXSession.TenantId = null;

            UsingDbContext(context => { context.Roles.Count().ShouldBe(0); });

            await CreateRoleAsync("HostRole1");

            UsingDbContext(context => { context.Roles.Count().ShouldBe(1); });

            //Switch to tenant 1
            StudioXSession.TenantId = 1;

            UsingDbContext(context => { context.Roles.Count().ShouldBe(0); });

            await CreateRoleAsync("TenantRole1");

            UsingDbContext(context => { context.Roles.Count().ShouldBe(1); });
        }

        [Fact]
        public async Task MultiTenancyTestsUsingUOW()
        {
            var uowManager = Resolve<IUnitOfWorkManager>();
            using (var uow = uowManager.Begin())
            {
                using (uowManager.Current.SetTenantId(null)) //Switch to host
                {
                    UsingDbContext(context => { context.Roles.Count().ShouldBe(0); });

                    await CreateRoleAsync("HostRole1");

                    UsingDbContext(context =>
                    {
                        context.Roles.Count().ShouldBe(1);
                        context.Roles.First().Name.ShouldBe("HostRole1");
                    });

                    using (uowManager.Current.SetTenantId(1)) //Switch to tenant 1
                    {
                        UsingDbContext(context => { context.Roles.Count().ShouldBe(0); });

                        await CreateRoleAsync("TenantRole1");

                        UsingDbContext(context =>
                        {
                            context.Roles.Count().ShouldBe(1);
                            context.Roles.First().Name.ShouldBe("TenantRole1");
                        });
                    }

                    //Automatically re-stored to host
                    UsingDbContext(context =>
                    {
                        context.Roles.Count().ShouldBe(1);
                        context.Roles.First().Name.ShouldBe("HostRole1");
                    });
                }

                await uow.CompleteAsync();
            }
        }

        protected async Task<Role> CreateRoleAsync(string name)
        {
            return await CreateRoleAsync(name, name);
        }

        protected async Task<Role> CreateRoleAsync(string name, string displayName)
        {
            var role = new Role(StudioXSession.TenantId, name, displayName);

            (await roleManager.CreateAsync(role)).Succeeded.ShouldBe(true);

            var uowManager = Resolve<IUnitOfWorkManager>();
            if (uowManager.Current != null)
            {
                await uowManager.Current.SaveChangesAsync();
            }

            await UsingDbContextAsync(async context =>
            {
                var createdRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == name);
                createdRole.ShouldNotBe(null);
            });

            return role;
        }
    }
}