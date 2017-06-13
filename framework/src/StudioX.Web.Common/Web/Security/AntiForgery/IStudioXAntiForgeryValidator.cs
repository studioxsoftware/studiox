namespace StudioX.Web.Security.AntiForgery
{
    /// <summary>
    /// This interface is internally used by StudioX framework and normally should not be used by applications.
    /// If it's needed, use 
    /// <see cref="IStudioXAntiForgeryManager"/> and cast to 
    /// <see cref="IStudioXAntiForgeryValidator"/> to use 
    /// <see cref="IsValid"/> method.
    /// </summary>
    public interface IStudioXAntiForgeryValidator
    {
        bool IsValid(string cookieValue, string tokenValue);
    }
}