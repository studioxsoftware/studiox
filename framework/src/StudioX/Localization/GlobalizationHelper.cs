using System.Globalization;
using StudioX.Extensions;

namespace StudioX.Localization
{
    internal static class GlobalizationHelper
    {
        public static bool IsValidCultureCode(string cultureCode)
        {
            if (cultureCode.IsNullOrWhiteSpace())
            {
                return false;
            }

            try
            {
                CultureInfoHelper.Get(cultureCode);
                return true;
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }
    }
}
