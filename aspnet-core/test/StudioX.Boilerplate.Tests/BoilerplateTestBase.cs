using System;
using System.Linq;
using System.Threading.Tasks;
using StudioX;
using StudioX.Authorization.Users;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Entities;
using StudioX.Runtime.Session;
using StudioX.TestBase;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.EntityFrameworkCore;
using StudioX.Boilerplate.EntityFrameworkCore.Seed.Host;
using StudioX.Boilerplate.EntityFrameworkCore.Seed.Tenants;
using StudioX.Boilerplate.MultiTenancy;
using Microsoft.EntityFrameworkCore;


namespace StudioX.Boilerplate.Tests
{
    public abstract class BoilerplateTestBase : StudioXIntegratedTestBase<BoilerplateTestModule>
    {
        protected BoilerplateTestBase()
        {
            void NormalizeDbContext(BoilerplateDbContext context)
            {
                context.EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
                context.EventBus = NullEventBus.Instance;
                context.SuppressAutoSetTenantId = true;
            }

            //Seed initial data for host
            StudioXSession.TenantId = null;
            UsingDbContext(context =>
            {
                NormalizeDbContext(context);
                new InitialHostDbBuilder(context).Create();
                new DefaultTenantBuilder(context).Create();
            });

            //Seed initial data for default tenant
            StudioXSession.TenantId = 1;
            UsingDbContext(context =>
            {
                NormalizeDbContext(context);
                new TenantRoleAndUserBuilder(context, 1).Create();
            });

            LoginAsDefaultTenantAdmin();
        }

        #region UsingDbContext

        protected IDisposable UsingTenantId(int? tenantId)
        {
            var previousTenantId = StudioXSession.TenantId;
            StudioXSession.TenantId = tenantId;
            return new DisposeAction(() => StudioXSession.TenantId = previousTenantId);
        }

        protected void UsingDbContext(Action<BoilerplateDbContext> action)
        {
            UsingDbContext(StudioXSession.TenantId, action);
        }

        protected Task UsingDbContextAsync(Func<BoilerplateDbContext, Task> action)
        {
            return UsingDbContextAsync(StudioXSession.TenantId, action);
        }

        protected T UsingDbContext<T>(Func<BoilerplateDbContext, T> func)
        {
            return UsingDbContext(StudioXSession.TenantId, func);
        }

        protected Task<T> UsingDbContextAsync<T>(Func<BoilerplateDbContext, Task<T>> func)
        {
            return UsingDbContextAsync(StudioXSession.TenantId, func);
        }

        protected void UsingDbContext(int? tenantId, Action<BoilerplateDbContext> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<BoilerplateDbContext>())
                {
                    action(context);
                    context.SaveChanges();
                }
            }
        }

        protected async Task UsingDbContextAsync(int? tenantId, Func<BoilerplateDbContext, Task> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<BoilerplateDbContext>())
                {
                    await action(context);
                    await context.SaveChangesAsync();
                }
            }
        }

        protected T UsingDbContext<T>(int? tenantId, Func<BoilerplateDbContext, T> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<BoilerplateDbContext>())
                {
                    result = func(context);
                    context.SaveChanges();
                }
            }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(int? tenantId, Func<BoilerplateDbContext, Task<T>> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<BoilerplateDbContext>())
                {
                    result = await func(context);
                    await context.SaveChangesAsync();
                }
            }

            return result;
        }

        #endregion

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

            var user =
                UsingDbContext(
                    context =>
                        context.Users.FirstOrDefault(u => u.TenantId == StudioXSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for host.");
            }

            StudioXSession.UserId = user.Id;
        }

        protected void LoginAsTenant(string tenancyName, string userName)
        {
            var tenant = UsingDbContext(context => context.Tenants.FirstOrDefault(t => t.TenancyName == tenancyName));
            if (tenant == null)
            {
                throw new Exception("There is no tenant: " + tenancyName);
            }

            StudioXSession.TenantId = tenant.Id;

            var user =
                UsingDbContext(
                    context =>
                        context.Users.FirstOrDefault(u => u.TenantId == StudioXSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for tenant: " + tenancyName);
            }

            StudioXSession.UserId = user.Id;
        }

        #endregion

        /// <summary>
        /// Gets current user if <see cref="IStudioXSession.UserId"/> is not null.
        /// Throws exception if it's null.
        /// </summary>
        protected async Task<User> GetCurrentUserAsync()
        {
            var userId = StudioXSession.GetUserId();
            return await UsingDbContext(context => context.Users.SingleAsync(u => u.Id == userId));
        }

        /// <summary>
        /// Gets current tenant if <see cref="IStudioXSession.TenantId"/> is not null.
        /// Throws exception if there is no current tenant.
        /// </summary>
        protected async Task<Tenant> GetCurrentTenantAsync()
        {
            var tenantId = StudioXSession.GetTenantId();
            return await UsingDbContext(context => context.Tenants.SingleAsync(t => t.Id == tenantId));
        }
    }
}
