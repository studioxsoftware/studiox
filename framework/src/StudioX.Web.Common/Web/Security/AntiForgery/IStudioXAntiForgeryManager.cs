namespace StudioX.Web.Security.AntiForgery
{
    public interface IStudioXAntiForgeryManager
    {
        IStudioXAntiForgeryConfiguration Configuration { get; }

        string GenerateToken();
    }
}
