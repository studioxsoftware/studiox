namespace StudioX.Auditing
{
    public interface IAuditSerializer
    {
        string Serialize(object obj);
    }
}