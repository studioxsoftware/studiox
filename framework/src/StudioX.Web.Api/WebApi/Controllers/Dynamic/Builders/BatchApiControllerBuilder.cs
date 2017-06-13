using System;
using System.Linq;
using System.Reflection;
using System.Web.Http.Filters;
using StudioX.Application.Services;
using StudioX.Dependency;
using StudioX.Extensions;

namespace StudioX.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// This interface is used to define a dynamic api controllers.
    /// </summary>
    /// <typeparam name="T">Type of the proxied object</typeparam>
    internal class BatchApiControllerBuilder<T> : IBatchApiControllerBuilder<T>
    {
        private readonly string servicePrefix;
        private readonly Assembly assembly;
        private IFilter[] filters;
        private Func<Type, string> serviceNameSelector;
        private Func<Type, bool> typePredicate;
        private bool conventionalVerbs;
        private Action<IApiControllerActionBuilder<T>> forMethodsAction;
        private bool? isApiExplorerEnabled;
        private readonly IIocResolver iocResolver;
        private readonly IDynamicApiControllerBuilder dynamicApiControllerBuilder;
        private bool? isProxyScriptingEnabled;

        public BatchApiControllerBuilder(
            IIocResolver iocResolver,
            IDynamicApiControllerBuilder dynamicApiControllerBuilder, 
            Assembly assembly, 
            string servicePrefix)
        {
            this.iocResolver = iocResolver;
            this.dynamicApiControllerBuilder = dynamicApiControllerBuilder;
            this.assembly = assembly;
            this.servicePrefix = servicePrefix;
        }

        public IBatchApiControllerBuilder<T> Where(Func<Type, bool> predicate)
        {
            typePredicate = predicate;
            return this;
        }

        public IBatchApiControllerBuilder<T> WithFilters(params IFilter[] filters)
        {
            this.filters = filters;
            return this;
        }

        public IBatchApiControllerBuilder<T> WithApiExplorer(bool isEnabled)
        {
            isApiExplorerEnabled = isEnabled;
            return this;
        }

        public IBatchApiControllerBuilder<T> WithProxyScripts(bool isEnabled)
        {
            isProxyScriptingEnabled = isEnabled;
            return this;
        }

        public IBatchApiControllerBuilder<T> WithServiceName(Func<Type, string> serviceNameSelector)
        {
            this.serviceNameSelector = serviceNameSelector;
            return this;
        }

        public IBatchApiControllerBuilder<T> ForMethods(Action<IApiControllerActionBuilder> action)
        {
            forMethodsAction = action;
            return this;
        }

        public IBatchApiControllerBuilder<T> WithConventionalVerbs()
        {
            conventionalVerbs = true;
            return this;
        }

        public void Build()
        {
            var types =
                from
                    type in assembly.GetTypes()
                where
                    (type.IsPublic || type.IsNestedPublic) && 
                    type.IsInterface && 
                    typeof(T).IsAssignableFrom(type) &&
                    iocResolver.IsRegistered(type) &&
                    !RemoteServiceAttribute.IsExplicitlyDisabledFor(type)
                select
                    type;

            if (typePredicate != null)
            {
                types = types.Where(t => typePredicate(t));
            }

            foreach (var type in types)
            {
                var serviceName = serviceNameSelector != null
                    ? serviceNameSelector(type)
                    : GetConventionalServiceName(type);

                if (!string.IsNullOrWhiteSpace(servicePrefix))
                {
                    serviceName = servicePrefix + "/" + serviceName;
                }

                var builder = typeof(IDynamicApiControllerBuilder)
                    .GetMethod("For", BindingFlags.Public | BindingFlags.Instance)
                    .MakeGenericMethod(type)
                    .Invoke(dynamicApiControllerBuilder, new object[] { serviceName });

                if (filters != null)
                {
                    builder.GetType()
                        .GetMethod("WithFilters", BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(builder, new object[] { filters });
                }

                if (isApiExplorerEnabled != null)
                {
                    builder.GetType()
                        .GetMethod("WithApiExplorer", BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(builder, new object[] { isApiExplorerEnabled });
                }

                if (isProxyScriptingEnabled != null)
                {
                    builder.GetType()
                        .GetMethod("WithProxyScripts", BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(builder, new object[] { isProxyScriptingEnabled.Value });
                }

                if (conventionalVerbs)
                {
                    builder.GetType()
                       .GetMethod("WithConventionalVerbs", BindingFlags.Public | BindingFlags.Instance)
                       .Invoke(builder, new object[0]);
                }

                if (forMethodsAction != null)
                {
                    builder.GetType()
                        .GetMethod("ForMethods", BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(builder, new object[] { forMethodsAction });
                }

                builder.GetType()
                        .GetMethod("Build", BindingFlags.Public | BindingFlags.Instance)
                        .Invoke(builder, new object[0]);
            }
        }
        
        public static string GetConventionalServiceName(Type type)
        {
            var typeName = type.Name;

            typeName = typeName.RemovePostFix(ApplicationService.CommonPostfixes);

            if (typeName.Length > 1 && typeName.StartsWith("I") && char.IsUpper(typeName, 1))
            {
                typeName = typeName.Substring(1);
            }

            return typeName.ToCamelCase();
        }
    }
}