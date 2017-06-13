using StudioX.Authorization;
using StudioX.MultiTenancy;

namespace StudioX.Boilerplate.Authorization.Modules
{
    public class SystemModuleAuthorizationProvider : ModuleAuthorizationProvider
    {
        public SystemModuleAuthorizationProvider(IPermissionDefinitionContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            GeneratePermissions();
        }

        private void GeneratePermissions()
        {
            var system = context.CreatePermission(PermissionNames.System.MainMenu, L(MenuText.System.MainMenu));

            var administration = system.CreateChildPermission(
                PermissionNames.System.Administration.MainMenu,
                L(MenuText.System.Administration.MainMenu)
            );

            GenerateTenantPermissions(administration);
            GenerateRolePermissions(administration);
            GenerateUserPermissions(administration);

            var configuration = system.CreateChildPermission(
                PermissionNames.System.Configuration.MainMenu,
                L(MenuText.System.Configuration.MainMenu)
            );

            GenerateAuditLogPermissions(configuration);
            GenerateSettingPermissions(configuration);
        }

        #region Constants

        private readonly IPermissionDefinitionContext context;

        private const string ReadOnlyTenant = "Read only";
        private const string CreatingNewTenant = "Creating new tenant";
        private const string DeletingTenant = "Deleting tenant";
        private const string EditingTenant = "Editing tenant";

        private const string ReadOnlyRole = "Read only";
        private const string CreatingNewRole = "Creating new role";
        private const string DeletingRole = "Deleting role";
        private const string EditingRole = "Editing role";

        private const string ReadOnlyUser = "Read only";
        private const string ChangePermission = "Change permissions";
        private const string CreatingNewUser = "Creating new user";
        private const string DeletingUser = "Deleting user";
        private const string EditingUser = "Editing user";

        private const string ReadOnlyAuditLog = "Read only";
        private const string ExportingAuditLog = "Exporting audit logs";

        private const string ReadOnlySetting = "Read only";
        private const string EditingSetting = "Editing setting";

        #endregion

        #region Administration

        private void GenerateTenantPermissions(Permission administration)
        {
            var tenants = administration.CreateChildPermission(
                PermissionNames.System.Administration.Tenants.MainMenu,
                L(MenuText.System.Administration.Tenants),
                multiTenancySides: MultiTenancySides.Host
            );

            tenants.CreateChildPermission(PermissionNames.System.Administration.Tenants.View, L(ReadOnlyTenant));
            tenants.CreateChildPermission(PermissionNames.System.Administration.Tenants.Create, L(CreatingNewTenant));
            tenants.CreateChildPermission(PermissionNames.System.Administration.Tenants.Delete, L(DeletingTenant));
            tenants.CreateChildPermission(PermissionNames.System.Administration.Tenants.Edit, L(EditingTenant));
        }

        private void GenerateRolePermissions(Permission administration)
        {
            var roles = administration.CreateChildPermission(
                PermissionNames.System.Administration.Roles.MainMenu,
                L(MenuText.System.Administration.Roles)
            );

            roles.CreateChildPermission(PermissionNames.System.Administration.Roles.View, L(ReadOnlyRole));
            roles.CreateChildPermission(PermissionNames.System.Administration.Roles.Create, L(CreatingNewRole));
            roles.CreateChildPermission(PermissionNames.System.Administration.Roles.Delete, L(DeletingRole));
            roles.CreateChildPermission(PermissionNames.System.Administration.Roles.Edit, L(EditingRole));
        }

        private void GenerateUserPermissions(Permission administration)
        {
            var users = administration.CreateChildPermission(
                PermissionNames.System.Administration.Users.MainMenu,
                L(MenuText.System.Administration.Users)
            );

            users.CreateChildPermission(PermissionNames.System.Administration.Users.View, L(ReadOnlyUser));
            users.CreateChildPermission(PermissionNames.System.Administration.Users.ChangePermission, L(ChangePermission));
            users.CreateChildPermission(PermissionNames.System.Administration.Users.Create, L(CreatingNewUser));
            users.CreateChildPermission(PermissionNames.System.Administration.Users.Delete, L(DeletingUser));
            users.CreateChildPermission(PermissionNames.System.Administration.Users.Edit, L(EditingUser));
        }

        #endregion

        #region Configuration

        private void GenerateAuditLogPermissions(Permission configuration)
        {
            var auditLogs = configuration.CreateChildPermission(
                PermissionNames.System.Configuration.AuditLogs.MainMenu,
                L(MenuText.System.Configuration.AuditLogs)
            );

            auditLogs.CreateChildPermission(PermissionNames.System.Configuration.AuditLogs.View, L(ReadOnlyAuditLog));
            auditLogs.CreateChildPermission(PermissionNames.System.Configuration.AuditLogs.Export,
                L(ExportingAuditLog));
        }

        private void GenerateSettingPermissions(Permission configuration)
        {
            var settings = configuration.CreateChildPermission(
                PermissionNames.System.Configuration.Settings.MainMenu,
                L(MenuText.System.Configuration.Settings),
                multiTenancySides: MultiTenancySides.Host
            );

            settings.CreateChildPermission(PermissionNames.System.Configuration.Settings.View, L(ReadOnlySetting));
            settings.CreateChildPermission(PermissionNames.System.Configuration.Settings.Edit, L(EditingSetting));
        }

        #endregion
    }
}