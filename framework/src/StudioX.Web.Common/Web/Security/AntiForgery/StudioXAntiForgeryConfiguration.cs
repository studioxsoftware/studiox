namespace StudioX.Web.Security.AntiForgery
{
    public class StudioXAntiForgeryConfiguration : IStudioXAntiForgeryConfiguration
    {
        public string TokenCookieName { get; set; }

        public string TokenHeaderName { get; set; }

        public StudioXAntiForgeryConfiguration()
        {
            TokenCookieName = "XSRF-TOKEN";
            TokenHeaderName = "X-XSRF-TOKEN";
        }
    }
}