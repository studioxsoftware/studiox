using System.Collections.Generic;
using System.Web.Http;
using StudioX.Domain.Uow;
using StudioX.Web.Models;
using StudioX.WebApi.Controllers.Dynamic.Builders;

namespace StudioX.WebApi.Configuration
{
    /// <summary>
    /// Used to configure StudioX WebApi module.
    /// </summary>
    public interface IStudioXWebApiConfiguration
    {
        /// <summary>
        /// Default UnitOfWorkAttribute for all actions.
        /// </summary>
        UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        /// <summary>
        /// Default WrapResultAttribute for all actions.
        /// </summary>
        WrapResultAttribute DefaultWrapResultAttribute { get; }

        /// <summary>
        /// Default WrapResultAttribute for all dynamic web api actions.
        /// </summary>
        WrapResultAttribute DefaultDynamicApiWrapResultAttribute { get; }

        /// <summary>
        /// List of URLs to ignore on result wrapping.
        /// </summary>
        List<string> ResultWrappingIgnoreUrls { get; }

        /// <summary>
        /// Gets/sets <see cref="HttpConfiguration"/>.
        /// </summary>
        HttpConfiguration HttpConfiguration { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        bool IsValidationEnabledForControllers { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        bool IsAutomaticAntiForgeryValidationEnabled { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        bool SetNoCacheForAllResponses { get; set; }

        /// <summary>
        /// Used to configure dynamic Web API controllers.
        /// </summary>
        IDynamicApiControllerBuilder DynamicApiControllerBuilder { get; }
    }
}