using System;
using StudioX.Dependency;
using Castle.MicroKernel.Registration;
using Microsoft.EntityFrameworkCore;

namespace StudioX.EntityFrameworkCore.Configuration
{
    public class StudioXEfCoreConfiguration : IStudioXEfCoreConfiguration
    {
        private readonly IIocManager _iocManager;

        public StudioXEfCoreConfiguration(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }

        public void AddDbContext<TDbContext>(Action<StudioXDbContextConfiguration<TDbContext>> action) 
            where TDbContext : DbContext
        {
            _iocManager.IocContainer.Register(
                Component.For<IStudioXDbContextConfigurer<TDbContext>>().Instance(
                    new StudioXDbContextConfigurerAction<TDbContext>(action)
                ).IsDefault()
            );
        }
    }
}