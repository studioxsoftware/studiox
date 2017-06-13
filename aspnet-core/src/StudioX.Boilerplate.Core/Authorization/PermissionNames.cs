namespace StudioX.Boilerplate.Authorization
{
    public static class PermissionNames
    {
        #region Dashboard
        public static class Pages
        {
            public const string Home = "Pages.Home";
        }
        #endregion

        #region System
        public static class System
        {
            public const string MainMenu = "System";

            public static class Administration
            {
                public const string MainMenu = "System.Administration";

                public static class Tenants
                {
                    public const string MainMenu = "System.Administration.Tenants";
                    public const string View = "System.Administration.Tenants.View";
                    public const string Create = "System.Administration.Tenants.Create";
                    public const string Edit = "System.Administration.Tenants.Edit";
                    public const string Delete = "System.Administration.Tenants.Delete";
                }

                public static class Roles
                {
                    public const string MainMenu = "System.Administration.Roles";
                    public const string View = "System.Administration.Roles.View";
                    public const string Create = "System.Administration.Roles.Create";
                    public const string Edit = "System.Administration.Roles.Edit";
                    public const string Delete = "System.Administration.Roles.Delete";
                }

                public static class Users
                {
                    public const string MainMenu = "System.Administration.Users";
                    public const string View = "System.Administration.Users.View";
                    public const string ChangePermission = "System.Administration.Users.ChangePermission";
                    public const string Create = "System.Administration.Users.Create";
                    public const string Edit = "System.Administration.Users.Edit";
                    public const string Delete = "System.Administration.Users.Delete";
                }
            }

            public static class Configuration
            {
                public const string MainMenu = "System.Configuration";

                public static class AuditLogs
                {
                    public const string MainMenu = "System.Configuration.AuditLogs";
                    public const string View = "System.Configuration.AuditLogs.View";
                    public const string Export = "System.Configuration.AuditLogs.Export";
                }

                public static class Settings
                {
                    public const string MainMenu = "System.Configuration.Settings";
                    public const string View = "System.Configuration.Settings.View";
                    public const string Edit = "System.Configuration.Settings.Edit";
                }
            }
        }

        #endregion
    }
}