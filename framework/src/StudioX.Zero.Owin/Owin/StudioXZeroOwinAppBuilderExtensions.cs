using StudioX.Authorization.Users;
using StudioX.Dependency;
using StudioX.Extensions;
using Microsoft.Owin.Security.DataProtection;
using Owin;

namespace StudioX.Owin
{
    public static class StudioXZeroOwinAppBuilderExtensions
    {
        public static void RegisterDataProtectionProvider(this IAppBuilder app)
        {
            if (!IocManager.Instance.IsRegistered<IUserTokenProviderAccessor>())
            {
                throw new StudioXException("IUserTokenProviderAccessor is not registered!");
            }

            var providerAccessor = IocManager.Instance.Resolve<IUserTokenProviderAccessor>();
            if (!(providerAccessor is OwinUserTokenProviderAccessor))
            {
                throw new StudioXException($"IUserTokenProviderAccessor should be instance of {nameof(OwinUserTokenProviderAccessor)}!");
            }

            providerAccessor.As<OwinUserTokenProviderAccessor>().DataProtectionProvider = app.GetDataProtectionProvider();
        }
    }
}