using System;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Authorization.Users;
using StudioX.EntityFrameworkCore.Extensions;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Entities;
using StudioX.Runtime.Session;
using StudioX.TestBase;
using StudioX.Zero.TestData;
using StudioX.ZeroCore.SampleApp.Core;
using StudioX.ZeroCore.SampleApp.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace StudioX.Zero
{
    public abstract class StudioXZeroTestBase : StudioXIntegratedTestBase<StudioXZeroTestModule>
    {
        protected StudioXZeroTestBase()
        {
            SeedTestData();
            LoginAsDefaultTenantAdmin();
        }

        private void SeedTestData()
        {
            void NormalizeDbContext(SampleAppDbContext context)
            {
                context.EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
                context.EventBus = NullEventBus.Instance;
                context.SuppressAutoSetTenantId = true;
            }

            //Seed initial data for default tenant
            StudioXSession.TenantId = 1;
            UsingDbContext(context =>
            {
                NormalizeDbContext(context);
                new TestDataBuilder(context, 1).Create();
            });
        }

        protected IDisposable UsingTenantId(int? tenantId)
        {
            var previousTenantId = StudioXSession.TenantId;
            StudioXSession.TenantId = tenantId;
            return new DisposeAction(() => StudioXSession.TenantId = previousTenantId);
        }

        protected void UsingDbContext(Action<SampleAppDbContext> action)
        {
            UsingDbContext(StudioXSession.TenantId, action);
        }

        protected Task UsingDbContextAsync(Func<SampleAppDbContext, Task> action)
        {
            return UsingDbContextAsync(StudioXSession.TenantId, action);
        }

        protected T UsingDbContext<T>(Func<SampleAppDbContext, T> func)
        {
            return UsingDbContext(StudioXSession.TenantId, func);
        }

        protected Task<T> UsingDbContextAsync<T>(Func<SampleAppDbContext, Task<T>> func)
        {
            return UsingDbContextAsync(StudioXSession.TenantId, func);
        }

        protected void UsingDbContext(int? tenantId, Action<SampleAppDbContext> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<SampleAppDbContext>())
                {
                    action(context);
                    context.SaveChanges();
                }
            }
        }

        protected async Task UsingDbContextAsync(int? tenantId, Func<SampleAppDbContext, Task> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<SampleAppDbContext>())
                {
                    await action(context);
                    await context.SaveChangesAsync();
                }
            }
        }

        protected T UsingDbContext<T>(int? tenantId, Func<SampleAppDbContext, T> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<SampleAppDbContext>())
                {
                    result = func(context);
                    context.SaveChanges();
                }
            }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(int? tenantId, Func<SampleAppDbContext, Task<T>> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<SampleAppDbContext>())
                {
                    result = await func(context);
                    await context.SaveChangesAsync();
                }
            }

            return result;
        }

        #region Login

        protected void LoginAsHostAdmin()
        {
            LoginAsHost(StudioXUserBase.AdminUserName);
        }

        protected void LoginAsDefaultTenantAdmin()
        {
            LoginAsTenant(Tenant.DefaultTenantName, StudioXUserBase.AdminUserName);
        }

        protected void LoginAsHost(string userName)
        {
            StudioXSession.TenantId = null;

            var user = UsingDbContext(context => context.Users.FirstOrDefault(u => u.TenantId == StudioXSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for host.");
            }

            StudioXSession.UserId = user.Id;
        }

        protected void LoginAsTenant(string tenancyName, string userName)
        {
            StudioXSession.TenantId = null;

            var tenant = UsingDbContext(context => context.Tenants.FirstOrDefault(t => t.TenancyName == tenancyName));
            if (tenant == null)
            {
                throw new Exception("There is no tenant: " + tenancyName);
            }

            StudioXSession.TenantId = tenant.Id;

            var user = UsingDbContext(context => context.Users.FirstOrDefault(u => u.TenantId == StudioXSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for tenant: " + tenancyName);
            }

            StudioXSession.UserId = user.Id;
        }

        #endregion

        #region GetCurrentUser

        /// <summary>
        /// Gets current user if <see cref="IStudioXSession.UserId"/> is not null.
        /// Throws exception if it's null.
        /// </summary>
        protected User GetCurrentUser()
        {
            var userId = StudioXSession.GetUserId();
            return UsingDbContext(context => context.Users.Single(u => u.Id == userId));
        }

        /// <summary>
        /// Gets current user if <see cref="IStudioXSession.UserId"/> is not null.
        /// Throws exception if it's null.
        /// </summary>
        protected async Task<User> GetCurrentUserAsync()
        {
            var userId = StudioXSession.GetUserId();
            return await UsingDbContext(context => context.Users.SingleAsync(u => u.Id == userId));
        }

        #endregion

        #region GetCurrentTenant

        /// <summary>
        /// Gets current tenant if <see cref="IStudioXSession.TenantId"/> is not null.
        /// Throws exception if there is no current tenant.
        /// </summary>
        protected Tenant GetCurrentTenant()
        {
            var tenantId = StudioXSession.GetTenantId();
            return UsingDbContext(null, context => context.Tenants.Single(t => t.Id == tenantId));
        }

        /// <summary>
        /// Gets current tenant if <see cref="IStudioXSession.TenantId"/> is not null.
        /// Throws exception if there is no current tenant.
        /// </summary>
        protected async Task<Tenant> GetCurrentTenantAsync()
        {
            var tenantId = StudioXSession.GetTenantId();
            return await UsingDbContext(null, context => context.Tenants.SingleAsync(t => t.Id == tenantId));
        }

        #endregion

        #region GetTenant / GetTenantOrNull

        protected Tenant GetTenant(string tenancyName)
        {
            return UsingDbContext(null, context => context.Tenants.Single(t => t.TenancyName == tenancyName));
        }

        protected async Task<Tenant> GetTenantAsync(string tenancyName)
        {
            return await UsingDbContext(null, async context => await context.Tenants.SingleAsync(t => t.TenancyName == tenancyName));
        }

        protected Tenant GetTenantOrNull(string tenancyName)
        {
            return UsingDbContext(null, context => context.Tenants.FirstOrDefault(t => t.TenancyName == tenancyName));
        }

        protected async Task<Tenant> GetTenantOrNullAsync(string tenancyName)
        {
            return await UsingDbContext(null, async context => await context.Tenants.FirstOrDefaultAsync(t => t.TenancyName == tenancyName));
        }

        #endregion

        #region GetRole

        protected Role GetRole(string roleName)
        {
            return UsingDbContext(context => context.Roles.Single(r => r.Name == roleName && r.TenantId == StudioXSession.TenantId));
        }

        protected async Task<Role> GetRoleAsync(string roleName)
        {
            return await UsingDbContext(async context => await context.Roles.SingleAsync(r => r.Name == roleName && r.TenantId == StudioXSession.TenantId));
        }

        #endregion

        #region GetUserByUserName

        protected User GetUserByUserName(string userName)
        {
            var user = GetUserByUserNameOrNull(userName);
            if (user == null)
            {
                throw new Exception("Can not find a user with username: " + userName);
            }

            return user;
        }

        protected async Task<User> GetUserByUserNameAsync(string userName)
        {
            var user = await GetUserByUserNameOrNullAsync(userName);
            if (user == null)
            {
                throw new Exception("Can not find a user with username: " + userName);
            }

            return user;
        }

        protected User GetUserByUserNameOrNull(string userName)
        {
            return UsingDbContext(context =>
                context.Users.FirstOrDefault(u =>
                    u.UserName == userName &&
                    u.TenantId == StudioXSession.TenantId
                ));
        }

        protected async Task<User> GetUserByUserNameOrNullAsync(string userName, bool includeRoles = false)
        {
            return await UsingDbContextAsync(async context =>
                await context.Users
                    .IncludeIf(includeRoles, u => u.Roles)
                    .FirstOrDefaultAsync(u =>
                        u.UserName == userName &&
                        u.TenantId == StudioXSession.TenantId
                    ));
        }

        #endregion
    }
}
