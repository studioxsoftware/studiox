namespace StudioX.Zero.Configuration
{
    public static class StudioXZeroSettingNames
    {
        public static class UserManagement
        {
            /// <summary>
            /// "StudioX.Zero.UserManagement.IsEmailConfirmationRequiredForLogin".
            /// </summary>
            public const string IsEmailConfirmationRequiredForLogin = "StudioX.Zero.UserManagement.IsEmailConfirmationRequiredForLogin";

            public static class UserLockOut
            {
                /// <summary>
                /// "StudioX.Zero.UserManagement.UserLockOut.IsEnabled".
                /// </summary>
                public const string IsEnabled = "StudioX.Zero.UserManagement.UserLockOut.IsEnabled";

                /// <summary>
                /// "StudioX.Zero.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout".
                /// </summary>
                public const string MaxFailedAccessAttemptsBeforeLockout = "StudioX.Zero.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout";

                /// <summary>
                /// "StudioX.Zero.UserManagement.UserLockOut.DefaultAccountLockoutSeconds".
                /// </summary>
                public const string DefaultAccountLockoutSeconds = "StudioX.Zero.UserManagement.UserLockOut.DefaultAccountLockoutSeconds";
            }

            public static class TwoFactorLogin
            {
                /// <summary>
                /// "StudioX.Zero.UserManagement.TwoFactorLogin.IsEnabled".
                /// </summary>
                public const string IsEnabled = "StudioX.Zero.UserManagement.TwoFactorLogin.IsEnabled";

                /// <summary>
                /// "StudioX.Zero.UserManagement.TwoFactorLogin.IsEmailProviderEnabled".
                /// </summary>
                public const string IsEmailProviderEnabled = "StudioX.Zero.UserManagement.TwoFactorLogin.IsEmailProviderEnabled";

                /// <summary>
                /// "StudioX.Zero.UserManagement.TwoFactorLogin.IsSmsProviderEnabled".
                /// </summary>
                public const string IsSmsProviderEnabled = "StudioX.Zero.UserManagement.TwoFactorLogin.IsSmsProviderEnabled";

                /// <summary>
                /// "StudioX.Zero.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled".
                /// </summary>
                public const string IsRememberBrowserEnabled = "StudioX.Zero.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled";
            }

            public static class PasswordComplexity
            {
                /// <summary>
                /// "StudioX.Zero.UserManagement.PasswordComplexity.RequiredLength"
                /// </summary>
                public const string RequiredLength = "StudioX.Zero.UserManagement.PasswordComplexity.RequiredLength";

                /// <summary>
                /// "StudioX.Zero.UserManagement.PasswordComplexity.RequireNonAlphanumeric"
                /// </summary>
                public const string RequireNonAlphanumeric = "StudioX.Zero.UserManagement.PasswordComplexity.RequireNonAlphanumeric";

                /// <summary>
                /// "StudioX.Zero.UserManagement.PasswordComplexity.RequireLowercase"
                /// </summary>
                public const string RequireLowercase = "StudioX.Zero.UserManagement.PasswordComplexity.RequireLowercase";

                /// <summary>
                /// "StudioX.Zero.UserManagement.PasswordComplexity.RequireUppercase"
                /// </summary>
                public const string RequireUppercase = "StudioX.Zero.UserManagement.PasswordComplexity.RequireUppercase";

                /// <summary>
                /// "StudioX.Zero.UserManagement.PasswordComplexity.RequireDigit"
                /// </summary>
                public const string RequireDigit = "StudioX.Zero.UserManagement.PasswordComplexity.RequireDigit";
            }
        }

        public static class OrganizationUnits
        {
            /// <summary>
            /// "StudioX.Zero.OrganizationUnits.MaxUserMembershipCount".
            /// </summary>
            public const string MaxUserMembershipCount = "StudioX.Zero.OrganizationUnits.MaxUserMembershipCount";
        }
    }
}