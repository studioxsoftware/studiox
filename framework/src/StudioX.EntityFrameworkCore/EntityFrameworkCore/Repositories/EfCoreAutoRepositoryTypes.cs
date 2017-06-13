using StudioX.Domain.Repositories;

namespace StudioX.EntityFrameworkCore.Repositories
{
    public static class EfCoreAutoRepositoryTypes
    {
        public static AutoRepositoryTypesAttribute Default { get; }

        static EfCoreAutoRepositoryTypes()
        {
            Default = new AutoRepositoryTypesAttribute(
                typeof(IRepository<>),
                typeof(IRepository<,>),
                typeof(EfCoreRepositoryBase<,>),
                typeof(EfCoreRepositoryBase<,,>)
            );
        }
    }
}