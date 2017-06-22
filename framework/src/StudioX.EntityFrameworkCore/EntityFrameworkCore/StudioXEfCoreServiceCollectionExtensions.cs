﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudioX.EntityFrameworkCore.Configuration;

namespace StudioX.EntityFrameworkCore
{
    public static class StudioXEfCoreServiceCollectionExtensions
    {
        public static void AddStudioXDbContext<TDbContext>(
            this IServiceCollection services,
            Action<StudioXDbContextConfiguration<TDbContext>> action)
            where TDbContext : DbContext
        {
            services.AddSingleton(
                typeof(IStudioXDbContextConfigurer<TDbContext>),
                new StudioXDbContextConfigurerAction<TDbContext>(action)
            );
        }
    }
}