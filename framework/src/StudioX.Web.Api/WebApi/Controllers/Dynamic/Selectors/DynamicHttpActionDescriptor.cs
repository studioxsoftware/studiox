using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using StudioX.Collections.Extensions;
using StudioX.Reflection;
using StudioX.Web;
using StudioX.WebApi.Configuration;
using StudioX.Extensions;

namespace StudioX.WebApi.Controllers.Dynamic.Selectors
{
    internal static class DynamicApiDescriptorHelper
    {
        internal static ReadOnlyCollection<T> FilterType<T>(object[] objects) where T : class
        {
            int max = objects.Length;
            List<T> list = new List<T>(max);
            int idx = 0;
            for (int i = 0; i < max; i++)
            {
                T attr = objects[i] as T;
                if (attr != null)
                {
                    list.Add(attr);
                    idx++;
                }
            }
            list.Capacity = idx;

            return new ReadOnlyCollection<T>(list);
        }
    }

    public class DynamicHttpActionDescriptor : ReflectedHttpActionDescriptor
    {
        public override Collection<HttpMethod> SupportedHttpMethods { get; }

        private readonly DynamicApiActionInfo actionInfo;
        private readonly Lazy<Collection<IFilter>> filters;
        private readonly Lazy<Collection<HttpParameterDescriptor>> parameters;

        private readonly object[] attributes;
        private readonly object[] declaredOnlyAttributes;
        
        public DynamicHttpActionDescriptor(
            IStudioXWebApiConfiguration configuration,
            HttpControllerDescriptor controllerDescriptor,
            DynamicApiActionInfo actionInfo)
            : base(
                  controllerDescriptor,
                  actionInfo.Method)
        {
            this.actionInfo = actionInfo;
            SupportedHttpMethods = new Collection<HttpMethod> { actionInfo.Verb.ToHttpMethod() };

            Properties["__StudioXDynamicApiActionInfo"] = actionInfo;
            Properties["__StudioXDynamicApiDontWrapResultAttribute"] =
                ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault(
                    actionInfo.Method,
                    configuration.DefaultDynamicApiWrapResultAttribute
                );

            filters = new Lazy<Collection<IFilter>>(GetFiltersInternal, true);
            parameters = new Lazy<Collection<HttpParameterDescriptor>>(GetParametersInternal, true);

            declaredOnlyAttributes = this.actionInfo.Method.GetCustomAttributes(inherit: false);
            attributes = this.actionInfo.Method.GetCustomAttributes(inherit: true);
        }

        /// <summary>
        /// Overrides the GetFilters for the action and adds the Dynamic Action filters.
        /// </summary>
        /// <returns> The Collection of filters.</returns>
        public override Collection<IFilter> GetFilters()
        {
            return filters.Value;
        }

        public override Collection<T> GetCustomAttributes<T>(bool inherit)
        {
            object[] attributes = inherit ? this.attributes : declaredOnlyAttributes;
            return new Collection<T>(DynamicApiDescriptorHelper.FilterType<T>(attributes));
        }

        public override Collection<HttpParameterDescriptor> GetParameters()
        {
            return parameters.Value;
        }

        private Collection<IFilter> GetFiltersInternal()
        {
            if (actionInfo.Filters.IsNullOrEmpty())
            {
                return base.GetFilters();
            }

            var actionFilters = new Collection<IFilter>();

            foreach (var filter in actionInfo.Filters)
            {
                actionFilters.Add(filter);
            }

            foreach (var baseFilter in base.GetFilters())
            {
                actionFilters.Add(baseFilter);
            }

            return actionFilters;
        }

        private Collection<HttpParameterDescriptor> GetParametersInternal()
        {
            var parameters = base.GetParameters();

            if (actionInfo.Verb.IsIn(HttpVerb.Get, HttpVerb.Head))
            {
                foreach (var parameter in parameters)
                {
                    if (parameter.ParameterBinderAttribute == null)
                    {
                        parameter.ParameterBinderAttribute = new FromUriAttribute();
                    }
                }
            }

            return parameters;
        }
    }
}