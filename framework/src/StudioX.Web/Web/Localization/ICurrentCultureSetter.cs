using System.Web;

namespace StudioX.Web.Localization
{
    public interface ICurrentCultureSetter
    {
        void SetCurrentCulture(HttpContext httpContext);
    }
}
