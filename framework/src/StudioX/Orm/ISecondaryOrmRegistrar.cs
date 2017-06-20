using StudioX.Dependency;
using StudioX.Domain.Repositories;

namespace StudioX.Orm
{
    public interface ISecondaryOrmRegistrar
    {
        string OrmContextKey { get; }

        void RegisterRepositories(IIocManager iocManager, AutoRepositoryTypesAttribute defaultRepositoryTypes);
    }
}