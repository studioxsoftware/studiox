namespace StudioX.Zero.Configuration
{
    internal class StudioXZeroConfig : IStudioXZeroConfig
    {
        public IRoleManagementConfig RoleManagement => roleManagementConfig;
        private readonly IRoleManagementConfig roleManagementConfig;

        public IUserManagementConfig UserManagement => userManagementConfig;
        private readonly IUserManagementConfig userManagementConfig;

        public ILanguageManagementConfig LanguageManagement => languageManagement;
        private readonly ILanguageManagementConfig languageManagement;

        public IStudioXZeroEntityTypes EntityTypes => entityTypes;
        private readonly IStudioXZeroEntityTypes entityTypes;


        public StudioXZeroConfig(
            IRoleManagementConfig roleManagementConfig,
            IUserManagementConfig userManagementConfig,
            ILanguageManagementConfig languageManagement,
            IStudioXZeroEntityTypes entityTypes)
        {
            this.entityTypes = entityTypes;
            this.roleManagementConfig = roleManagementConfig;
            this.userManagementConfig = userManagementConfig;
            this.languageManagement = languageManagement;
        }
    }
}