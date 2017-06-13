using System.Globalization;
using StudioX.Configuration.Startup;
using StudioX.Extensions;
using StudioX.Logging;
using Castle.Core.Logging;

namespace StudioX.Localization
{
    public static class LocalizationSourceHelper
    {
        public static string ReturnGivenNameOrThrowException(
            ILocalizationConfiguration configuration,
            string sourceName, 
            string name, 
            CultureInfo culture,
            ILogger logger = null)
        {
            var exceptionMessage = $"Can not find '{name}' in localization source '{sourceName}'!";

            if (!configuration.ReturnGivenTextIfNotFound)
            {
                throw new StudioXException(exceptionMessage);
            }

            if (configuration.LogWarnMessageIfNotFound)
            {
                (logger ?? LogHelper.Logger).Warn(exceptionMessage);
            }

            string notFoundText;
#if NET46
            notFoundText = configuration.HumanizeTextIfNotFound
                ? name.ToSentenceCase(culture)
                : name;
#else
            using (CultureInfoHelper.Use(culture))
            {
                notFoundText = configuration.HumanizeTextIfNotFound
                    ? name.ToSentenceCase()
                    : name;
            }
#endif

            return configuration.WrapGivenTextIfNotFound
                ? $"[{notFoundText}]"
                : notFoundText;
        }
    }
}
