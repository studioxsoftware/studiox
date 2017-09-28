using StudioX.Dependency;
using StudioX.Modules;
using StudioX.Orm;
using StudioX.Reflection.Extensions;

namespace StudioX.Dapper
{
    [DependsOn(typeof(StudioXKernelModule))]
    public class StudioXDapperModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactionScopeAvailable = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXDapperModule).GetAssembly());

            using (IScopedIocResolver scope = IocManager.CreateScope())
            {
                ISecondaryOrmRegistrar[] additionalOrmRegistrars = scope.ResolveAll<ISecondaryOrmRegistrar>();

                foreach (ISecondaryOrmRegistrar registrar in additionalOrmRegistrars)
                {
                    if (registrar.OrmContextKey == StudioXConsts.Orms.EntityFramework)
                    {
                        registrar.RegisterRepositories(IocManager, EfBasedDapperAutoRepositoryTypes.Default);
                    }

                    if (registrar.OrmContextKey == StudioXConsts.Orms.NHibernate)
                    {
                        registrar.RegisterRepositories(IocManager, NhBasedDapperAutoRepositoryTypes.Default);
                    }

                    if (registrar.OrmContextKey == StudioXConsts.Orms.EntityFrameworkCore)
                    {
                        registrar.RegisterRepositories(IocManager, EfBasedDapperAutoRepositoryTypes.Default);
                    }
                }
            }
        }
    }
}
