namespace StudioX.MongoDb.Configuration
{
    internal class StudioXMongoDbModuleConfiguration : IStudioXMongoDbModuleConfiguration
    {
        public string ConnectionString { get; set; }

        public string DatatabaseName { get; set; }
    }
}