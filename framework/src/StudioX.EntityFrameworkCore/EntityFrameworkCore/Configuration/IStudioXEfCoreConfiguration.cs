using System;
using Microsoft.EntityFrameworkCore;

namespace StudioX.EntityFrameworkCore.Configuration
{
    public interface IStudioXEfCoreConfiguration
    {
        void AddDbContext<TDbContext>(Action<StudioXDbContextConfiguration<TDbContext>> action)
            where TDbContext : DbContext;
    }
}
