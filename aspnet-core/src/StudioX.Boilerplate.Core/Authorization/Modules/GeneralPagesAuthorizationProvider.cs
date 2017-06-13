using StudioX.Authorization;

namespace StudioX.Boilerplate.Authorization.Modules
{
    public class GeneralPagesAuthorizationProvider : ModuleAuthorizationProvider
    {
        private readonly IPermissionDefinitionContext context;

        public GeneralPagesAuthorizationProvider(IPermissionDefinitionContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            GeneratePermissions();
        }

        private void GeneratePermissions()
        {
            var dashboard = context.GetPermissionOrNull(PermissionNames.Pages.Home) ??
                            context.CreatePermission(PermissionNames.Pages.Home, L(MenuText.Page.Home));
        }
    }
}
