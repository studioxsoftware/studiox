using System;

namespace StudioX.Web.Security.AntiForgery
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class DisableStudioXAntiForgeryTokenValidationAttribute : Attribute
    {

    }
}
