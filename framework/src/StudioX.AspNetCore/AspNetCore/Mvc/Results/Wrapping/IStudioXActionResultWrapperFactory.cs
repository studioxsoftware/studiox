using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Results.Wrapping
{
    public interface IStudioXActionResultWrapperFactory
    {
        IStudioXActionResultWrapper CreateFor([NotNull] ResultExecutingContext actionResult);
    }
}