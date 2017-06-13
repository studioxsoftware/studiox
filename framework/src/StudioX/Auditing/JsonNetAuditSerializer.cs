using StudioX.Dependency;
using Newtonsoft.Json;

namespace StudioX.Auditing
{
    public class JsonNetAuditSerializer : IAuditSerializer, ITransientDependency
    {
        private readonly IAuditingConfiguration configuration;

        public JsonNetAuditSerializer(IAuditingConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Serialize(object obj)
        {
            var options = new JsonSerializerSettings
            {
                ContractResolver = new AuditingContractResolver(configuration.IgnoredTypes)
            };

            return JsonConvert.SerializeObject(obj, options);
        }
    }
}