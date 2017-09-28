using System.Configuration;
using StudioX.Configuration.Startup;
using StudioX.Extensions;

namespace StudioX.Runtime.Caching.Redis
{
    public class StudioXRedisCacheOptions
    {
        public IStudioXStartupConfiguration StudioXStartupConfiguration { get; }

        private const string ConnectionStringKey = "StudioX.Redis.Cache";

        private const string DatabaseIdSettingKey = "StudioX.Redis.Cache.DatabaseId";

        public string ConnectionString { get; set; }

        public int DatabaseId { get; set; }

        public StudioXRedisCacheOptions(IStudioXStartupConfiguration studioxStartupConfiguration)
        {
            StudioXStartupConfiguration = studioxStartupConfiguration;

            ConnectionString = GetDefaultConnectionString();
            DatabaseId = GetDefaultDatabaseId();
        }

        private static int GetDefaultDatabaseId()
        {
            var appSetting = ConfigurationManager.AppSettings[DatabaseIdSettingKey];
            if (appSetting.IsNullOrEmpty())
            {
                return -1;
            }

            int databaseId;
            if (!int.TryParse(appSetting, out databaseId))
            {
                return -1;
            }

            return databaseId;
        }

        private static string GetDefaultConnectionString()
        {
            var connStr = ConfigurationManager.ConnectionStrings[ConnectionStringKey];
            if (connStr == null || connStr.ConnectionString.IsNullOrWhiteSpace())
            {
                return "localhost";
            }

            return connStr.ConnectionString;
        }
    }
}