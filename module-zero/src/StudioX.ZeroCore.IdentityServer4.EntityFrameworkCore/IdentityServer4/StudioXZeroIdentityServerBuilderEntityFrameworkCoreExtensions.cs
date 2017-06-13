using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace StudioX.IdentityServer4
{
    public static class StudioXZeroIdentityServerBuilderEntityFrameworkCoreExtensions
    {
        public static IIdentityServerBuilder AddStudioXPersistedGrants<TDbContext>(this IIdentityServerBuilder builder)
            where TDbContext : IStudioXPersistedGrantDbContext
        {
            builder.Services.AddTransient<IPersistedGrantStore, StudioXPersistedGrantStore>();
            return builder;
        }
    }
}
