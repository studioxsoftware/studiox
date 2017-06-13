using System.Reflection;
using System.Web.Http.Controllers;
using StudioX.Extensions;

namespace StudioX.WebApi.Validation
{
    public static class ActionDescriptorExtensions
    {
        public static MethodInfo GetMethodInfoOrNull(this HttpActionDescriptor actionDescriptor)
        {
            if (actionDescriptor is ReflectedHttpActionDescriptor)
            {
                return actionDescriptor.As<ReflectedHttpActionDescriptor>().MethodInfo;
            }
            
            return null;
        }

        public static bool IsDynamicStudioXAction(this HttpActionDescriptor actionDescriptor)
        {
            return actionDescriptor
                .ControllerDescriptor
                .Properties
                .ContainsKey("__StudioXDynamicApiControllerInfo");
        }
    }
}