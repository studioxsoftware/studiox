using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Results.Wrapping
{
    public interface IStudioXActionResultWrapper
    {
        void Wrap(ResultExecutingContext actionResult);
    }
}