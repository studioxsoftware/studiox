namespace StudioX.Net.Mail
{
    /// <summary>
    ///     Declares names of the settings defined by <see cref="EmailSettingProvider" />.
    /// </summary>
    public static class EmailSettingNames
    {
        /// <summary>
        ///     StudioX.Net.Mail.DefaultFromAddress
        /// </summary>
        public const string DefaultFromAddress = "StudioX.Net.Mail.DefaultFromAddress";

        /// <summary>
        ///     StudioX.Net.Mail.DefaultFromDisplayName
        /// </summary>
        public const string DefaultFromDisplayName = "StudioX.Net.Mail.DefaultFromDisplayName";

        /// <summary>
        ///     SMTP related email settings.
        /// </summary>
        public static class Smtp
        {
            /// <summary>
            ///     StudioX.Net.Mail.Smtp.Host
            /// </summary>
            public const string Host = "StudioX.Net.Mail.Smtp.Host";

            /// <summary>
            ///     StudioX.Net.Mail.Smtp.Port
            /// </summary>
            public const string Port = "StudioX.Net.Mail.Smtp.Port";

            /// <summary>
            ///     StudioX.Net.Mail.Smtp.UserName
            /// </summary>
            public const string UserName = "StudioX.Net.Mail.Smtp.UserName";

            /// <summary>
            ///     StudioX.Net.Mail.Smtp.Password
            /// </summary>
            public const string Password = "StudioX.Net.Mail.Smtp.Password";

            /// <summary>
            ///     StudioX.Net.Mail.Smtp.Domain
            /// </summary>
            public const string Domain = "StudioX.Net.Mail.Smtp.Domain";

            /// <summary>
            ///     StudioX.Net.Mail.Smtp.EnableSsl
            /// </summary>
            public const string EnableSsl = "StudioX.Net.Mail.Smtp.EnableSsl";

            /// <summary>
            ///     StudioX.Net.Mail.Smtp.UseDefaultCredentials
            /// </summary>
            public const string UseDefaultCredentials = "StudioX.Net.Mail.Smtp.UseDefaultCredentials";
        }
    }
}