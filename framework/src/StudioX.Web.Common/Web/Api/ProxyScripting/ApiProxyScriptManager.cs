using System.Collections.Concurrent;
using StudioX.Collections.Extensions;
using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.Json;
using StudioX.Web.Api.Modeling;
using StudioX.Web.Api.ProxyScripting.Configuration;
using StudioX.Web.Api.ProxyScripting.Generators;

namespace StudioX.Web.Api.ProxyScripting
{
    public class ApiProxyScriptManager : IApiProxyScriptManager, ISingletonDependency
    {
        private readonly IApiDescriptionModelProvider modelProvider;
        private readonly IApiProxyScriptingConfiguration configuration;
        private readonly IIocResolver iocResolver;

        private readonly ConcurrentDictionary<string, string> cache;

        public ApiProxyScriptManager(
            IApiDescriptionModelProvider modelProvider, 
            IApiProxyScriptingConfiguration configuration,
            IIocResolver iocResolver)
        {
            this.modelProvider = modelProvider;
            this.configuration = configuration;
            this.iocResolver = iocResolver;

            cache = new ConcurrentDictionary<string, string>();
        }

        public string GetScript(ApiProxyGenerationOptions options)
        {
            if (options.UseCache)
            {
                return cache.GetOrAdd(CreateCacheKey(options), (key) => CreateScript(options));
            }

            return cache[CreateCacheKey(options)] = CreateScript(options);
        }

        private string CreateScript(ApiProxyGenerationOptions options)
        {
            var model = modelProvider.CreateModel();

            if (options.IsPartialRequest())
            {
                model = model.CreateSubModel(options.Modules, options.Controllers, options.Actions);
            }

            var generatorType = configuration.Generators.GetOrDefault(options.GeneratorType);
            if (generatorType == null)
            {
                throw new StudioXException($"Could not find a proxy script generator with given name: {options.GeneratorType}");
            }

            using (var generator = iocResolver.ResolveAsDisposable<IProxyScriptGenerator>(generatorType))
            {
                return generator.Object.CreateScript(model);
            }
        }

        private static string CreateCacheKey(ApiProxyGenerationOptions options)
        {
            return options.ToJsonString().ToMd5();
        }
    }
}
