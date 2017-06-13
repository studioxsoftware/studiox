namespace StudioX.Boilerplate.Authorization
{
    public static class MenuText
    {
        public static class Page
        {
            public const string Home = "Home";
        }
        
        public static class System
        {
            public const string MainMenu = "System";

            public static class Administration
            {
                public const string MainMenu = "Administration";
                public const string Roles = "Roles";
                public const string Tenants = "Tenants";
                public const string Users = "Users";
            }

            public static class Configuration
            {
                public const string MainMenu = "Configuration";
                public const string AuditLogs = "Audit logs";
                public const string Settings = "Settings";
            }
        }
    }
}
