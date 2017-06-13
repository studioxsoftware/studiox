using System;
using Microsoft.EntityFrameworkCore;

namespace StudioX.EntityFrameworkCore.Configuration
{
    public class StudioXDbContextConfigurerAction<TDbContext> : IStudioXDbContextConfigurer<TDbContext>
        where TDbContext : DbContext
    {
        public Action<StudioXDbContextConfiguration<TDbContext>> Action { get; set; }

        public StudioXDbContextConfigurerAction(Action<StudioXDbContextConfiguration<TDbContext>> action)
        {
            Action = action;
        }

        public void Configure(StudioXDbContextConfiguration<TDbContext> configuration)
        {
            Action(configuration);
        }
    }
}