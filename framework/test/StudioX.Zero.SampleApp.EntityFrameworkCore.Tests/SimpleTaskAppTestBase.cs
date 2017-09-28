using System;
using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.EntityFrameworkCore;
using StudioX.TestBase;
using StudioX.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.HostDatas;

namespace StudioX.Zero.SampleApp.EntityFrameworkCore.Tests
{
    public class SimpleTaskAppTestBase : StudioXIntegratedTestBase<SampleAppTestModule>
    {
        public SimpleTaskAppTestBase()
        {
            UsingDbContext(context => new HostDataBuilder(context).Build());
        }

        protected virtual void UsingDbContext(Action<AppDbContext> action)
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                using (var contextProvider = LocalIocManager.ResolveAsDisposable<IDbContextProvider<AppDbContext>>())
                {
                    var dbContext = contextProvider.Object.GetDbContext();

                    action(dbContext);
                    dbContext.SaveChanges(true);
                }

                uow.Complete();
            }
        }

        protected virtual T UsingDbContext<T>(Func<AppDbContext, T> func)
        {
            T result;

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                using (var contextProvider = LocalIocManager.ResolveAsDisposable<IDbContextProvider<AppDbContext>>())
                {
                    var dbContext = contextProvider.Object.GetDbContext();

                    result = func(dbContext);
                    dbContext.SaveChanges(true);
                }

                uow.Complete();
            }

            return result;
        }

        protected virtual async Task UsingDbContextAsync(Func<AppDbContext, Task> action)
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                using (var contextProvider = LocalIocManager.ResolveAsDisposable<IDbContextProvider<AppDbContext>>())
                {
                    var dbContext = contextProvider.Object.GetDbContext();                    

                    await action(dbContext);
                    await dbContext.SaveChangesAsync(true);
                }

                await uow.CompleteAsync();
            }
        }

        protected virtual async Task<T> UsingDbContextAsync<T>(Func<AppDbContext, Task<T>> func)
        {
            T result;

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                using (var contextProvider = LocalIocManager.ResolveAsDisposable<IDbContextProvider<AppDbContext>>())
                {
                    var dbContext = contextProvider.Object.GetDbContext();

                    result = await func(dbContext);
                    await dbContext.SaveChangesAsync(true);
                }

                await uow.CompleteAsync();
            }

            return result;
        }
    }
}