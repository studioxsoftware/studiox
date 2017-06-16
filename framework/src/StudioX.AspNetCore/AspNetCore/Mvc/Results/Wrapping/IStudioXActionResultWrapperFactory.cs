using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;
using StudioX.Dependency;

namespace StudioX.AspNetCore.Mvc.Results.Wrapping
{
    public interface IStudioXActionResultWrapperFactory : ITransientDependency
    {
        IStudioXActionResultWrapper CreateFor([NotNull] ResultExecutingContext actionResult);
    }
}