using System.Collections.Generic;
using StudioX.Application.Services;

namespace StudioX.WebApi.Controllers.Dynamic
{
    /// <summary>
    /// This class is used as base class for all dynamically created ApiControllers.  
    /// </summary>
    /// <typeparam name="T">Type of the proxied object</typeparam>
    /// <remarks>
    /// A dynamic ApiController is used to transparently expose an object (Generally an Application Service class)
    /// to remote clients.
    /// </remarks>
    public class DynamicApiController<T> : StudioXApiController, IDynamicApiController, IAvoidDuplicateCrossCuttingConcerns
    {
        public List<string> AppliedCrossCuttingConcerns { get; }

        public DynamicApiController()
        {
            AppliedCrossCuttingConcerns = new List<string>();
        }
    }
}