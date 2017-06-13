using System;
using System.Collections.Generic;

namespace StudioX.Configuration.Startup
{
    public interface IValidationConfiguration
    {
        List<Type> IgnoredTypes { get; }
    }
}