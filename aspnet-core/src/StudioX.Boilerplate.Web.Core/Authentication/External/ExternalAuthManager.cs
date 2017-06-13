using System;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Dependency;

namespace StudioX.Boilerplate.Authentication.External
{
    public class ExternalAuthManager : IExternalAuthManager, ITransientDependency
    {
        private readonly IIocResolver iocResolver;
        private readonly IExternalAuthConfiguration externalAuthConfiguration;

        public ExternalAuthManager(IIocResolver iocResolver, IExternalAuthConfiguration externalAuthConfiguration)
        {
            this.iocResolver = iocResolver;
            this.externalAuthConfiguration = externalAuthConfiguration;
        }

        public Task<bool> IsValidUser(string provider, string providerKey, string providerAccessCode)
        {
            using (var providerApi = CreateProviderApi(provider))
            {
                return providerApi.Object.IsValidUser(providerKey, providerAccessCode);
            }
        }

        public Task<ExternalAuthUserInfo> GetUserInfo(string provider, string accessCode)
        {
            using (var providerApi = CreateProviderApi(provider))
            {
                return providerApi.Object.GetUserInfo(accessCode);
            }
        }

        public IDisposableDependencyObjectWrapper<IExternalAuthProviderApi> CreateProviderApi(string provider)
        {
            var providerInfo = externalAuthConfiguration.Providers.FirstOrDefault(p => p.Name == provider);
            if (providerInfo == null)
            {
                throw new Exception("Unknown external auth provider: " + provider);
            }

            var providerApi = iocResolver.ResolveAsDisposable<IExternalAuthProviderApi>(providerInfo.ProviderApiType);
            providerApi.Object.Initialize(providerInfo);
            return providerApi;
        }
    }
}
