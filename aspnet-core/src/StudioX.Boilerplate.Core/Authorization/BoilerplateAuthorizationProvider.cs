using StudioX.Authorization;
using StudioX.Localization;
using StudioX.MultiTenancy;
using StudioX.Boilerplate.Authorization.Modules;

namespace StudioX.Boilerplate.Authorization
{
    public class BoilerplateAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            new GeneralPagesAuthorizationProvider(context).Create();
            new SystemModuleAuthorizationProvider(context).Create();
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, BoilerplateConsts.LocalizationSourceName);
        }
    }
}
