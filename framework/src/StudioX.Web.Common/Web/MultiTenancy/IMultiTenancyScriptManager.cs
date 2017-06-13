namespace StudioX.Web.MultiTenancy
{
    /// <summary>
    /// Used to create client scripts for multi-tenancy.
    /// </summary>
    public interface IMultiTenancyScriptManager
    {
        string GetScript();
    }
}