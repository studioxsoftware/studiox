using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using StudioX.Collections.Extensions;
using StudioX.Dependency;

namespace StudioX.WebApi.Controllers.Dynamic
{
    /// <summary>
    /// This class is used to store dynamic controller information.
    /// </summary>
    public class DynamicApiControllerManager : ISingletonDependency
    {
        private readonly IDictionary<string, DynamicApiControllerInfo> dynamicApiControllers;

        public DynamicApiControllerManager()
        {
            dynamicApiControllers = new Dictionary<string, DynamicApiControllerInfo>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Registers given controller info to be found later.
        /// </summary>
        /// <param name="controllerInfo">Controller info</param>
        public void Register(DynamicApiControllerInfo controllerInfo)
        {
            dynamicApiControllers[controllerInfo.ServiceName] = controllerInfo;
        }

        /// <summary>
        /// Searches and returns a dynamic api controller for given name.
        /// </summary>
        /// <param name="controllerName">Name of the controller</param>
        /// <returns>Controller info</returns>
        public DynamicApiControllerInfo FindOrNull(string controllerName)
        {
            return dynamicApiControllers.GetOrDefault(controllerName);
        }

        public IReadOnlyList<DynamicApiControllerInfo> GetAll()
        {
            return dynamicApiControllers.Values.ToImmutableList();
        }
    }
}