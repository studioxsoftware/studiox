namespace StudioX.MongoDb.Configuration
{
    public interface IStudioXMongoDbModuleConfiguration
    {
        string ConnectionString { get; set; }

        string DatatabaseName { get; set; }
    }
}
