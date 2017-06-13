using System;
using System.Linq;
using System.Net.Http;
using StudioX.Collections.Extensions;
using StudioX.Dependency;
using StudioX.Extensions;
using StudioX.TestBase.Runtime.Session;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace StudioX.AspNetCore.TestBase
{
    public abstract class StudioXAspNetCoreIntegratedTestBase<TStartup> 
        where TStartup : class
    {
        protected TestServer Server { get; }

        protected HttpClient Client { get; }

        protected IServiceProvider ServiceProvider { get; }

        protected IIocManager IocManager { get; }

        protected TestStudioXSession StudioXSession { get; }

        protected StudioXAspNetCoreIntegratedTestBase()
        {
            var builder = CreateWebHostBuilder();
            Server = CreateTestServer(builder);
            Client = Server.CreateClient();

            ServiceProvider = Server.Host.Services;
            IocManager = ServiceProvider.GetRequiredService<IIocManager>();
            StudioXSession = ServiceProvider.GetRequiredService<TestStudioXSession>();
        }

        protected virtual IWebHostBuilder CreateWebHostBuilder()
        {
            return new WebHostBuilder()
                .UseStartup<TStartup>();
        }

        protected virtual TestServer CreateTestServer(IWebHostBuilder builder)
        {
            return new TestServer(builder);
        }

        #region GetUrl

        /// <summary>
        /// Gets default URL for given controller type.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        protected virtual string GetUrl<TController>()
        {
            return "/" + typeof(TController).Name.RemovePostFix("Controller", "AppService", "ApplicationService", "Service");
        }

        /// <summary>
        /// Gets default URL for given controller type's given action.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        protected virtual string GetUrl<TController>(string actionName)
        {
            return GetUrl<TController>() + "/" + actionName;
        }

        /// <summary>
        /// Gets default URL for given controller type's given action with query string parameters (as anonymous object).
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        protected virtual string GetUrl<TController>(string actionName, object queryStringParamsAsAnonymousObject)
        {
            var url = GetUrl<TController>(actionName);

            var dictionary = new RouteValueDictionary(queryStringParamsAsAnonymousObject);
            if (dictionary.Any())
            {
                url += "?" + dictionary.Select(d => $"{d.Key}={d.Value}").JoinAsString("&");
            }

            return url;
        }

        #endregion
        
        #region Resolve

        /// <summary>
        /// A shortcut to resolve an object from <see cref="IocManager"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <returns>The object instance</returns>
        protected T Resolve<T>()
        {
            return IocManager.Resolve<T>();
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="IocManager"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object to get</typeparam>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected T Resolve<T>(object argumentsAsAnonymousType)
        {
            return IocManager.Resolve<T>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="IocManager"/>.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type)
        {
            return IocManager.Resolve(type);
        }

        /// <summary>
        /// A shortcut to resolve an object from <see cref="IocManager"/>.
        /// </summary>
        /// <param name="type">Type of the object to get</param>
        /// <param name="argumentsAsAnonymousType">Constructor arguments</param>
        /// <returns>The object instance</returns>
        protected object Resolve(Type type, object argumentsAsAnonymousType)
        {
            return IocManager.Resolve(type, argumentsAsAnonymousType);
        }

        #endregion
    }
}
