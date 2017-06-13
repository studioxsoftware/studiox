using System.Collections.Generic;
using System.Web.Http;
using StudioX.Domain.Uow;
using StudioX.Web.Models;
using StudioX.WebApi.Controllers.Dynamic.Builders;

namespace StudioX.WebApi.Configuration
{
    internal class StudioXWebApiConfiguration : IStudioXWebApiConfiguration
    {
        public UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        public WrapResultAttribute DefaultWrapResultAttribute { get; }

        public WrapResultAttribute DefaultDynamicApiWrapResultAttribute { get; }

        public List<string> ResultWrappingIgnoreUrls { get; }

        public HttpConfiguration HttpConfiguration { get; set; }

        public bool IsValidationEnabledForControllers { get; set; }

        public bool IsAutomaticAntiForgeryValidationEnabled { get; set; }

        public bool SetNoCacheForAllResponses { get; set; }

        public IDynamicApiControllerBuilder DynamicApiControllerBuilder { get; }

        public StudioXWebApiConfiguration(IDynamicApiControllerBuilder dynamicApiControllerBuilder)
        {
            DynamicApiControllerBuilder = dynamicApiControllerBuilder;

            HttpConfiguration = GlobalConfiguration.Configuration;
            DefaultUnitOfWorkAttribute = new UnitOfWorkAttribute();
            DefaultWrapResultAttribute = new WrapResultAttribute(false);
            DefaultDynamicApiWrapResultAttribute = new WrapResultAttribute();
            ResultWrappingIgnoreUrls = new List<string>();
            IsValidationEnabledForControllers = true;
            IsAutomaticAntiForgeryValidationEnabled = true;
            SetNoCacheForAllResponses = true;
        }
    }
}