using System;
using StudioX.Dependency;
using StudioX.Domain.Repositories;

namespace StudioX.EntityFramework.Repositories
{
    public interface IEfGenericRepositoryRegistrar
    {
        void RegisterForDbContext(Type dbContextType, IIocManager iocManager, AutoRepositoryTypesAttribute defaultAutoRepositoryTypesAttribute);
    }
}