using System.Reflection;
using StudioX.Reflection;

namespace StudioX.Web.Security.AntiForgery
{
    public static class StudioXAntiForgeryManagerWebExtensions
    {
        public static bool ShouldValidate(
            this IStudioXAntiForgeryManager manager,
            IStudioXAntiForgeryWebConfiguration antiForgeryWebConfiguration,
            MethodInfo methodInfo, 
            HttpVerb httpVerb, 
            bool defaultValue)
        {
            if (!antiForgeryWebConfiguration.IsEnabled)
            {
                return false;
            }

            if (methodInfo.IsDefined(typeof(ValidateStudioXAntiForgeryTokenAttribute), true))
            {
                return true;
            }

            if (ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableStudioXAntiForgeryTokenValidationAttribute>(methodInfo) != null)
            {
                return false;
            }

            if (antiForgeryWebConfiguration.IgnoredHttpVerbs.Contains(httpVerb))
            {
                return false;
            }

            if (methodInfo.DeclaringType?.IsDefined(typeof(ValidateStudioXAntiForgeryTokenAttribute), true) ?? false)
            {
                return true;
            }

            return defaultValue;
        }
    }
}
