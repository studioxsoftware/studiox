using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Results.Wrapping
{
    public static class StudioXActionResultWrapperFactory
    {
        public static IStudioXActionResultWrapper CreateFor([NotNull] ResultExecutingContext actionResult)
        {
            Check.NotNull(actionResult, nameof(actionResult));

            if (actionResult.Result is ObjectResult)
            {
                return new StudioXObjectActionResultWrapper();
            }

            if (actionResult.Result is JsonResult)
            {
                return new StudioXJsonActionResultWrapper();
            }

            if (actionResult.Result is EmptyResult)
            {
                return new StudioXEmptyActionResultWrapper();
            }

            return new NullStudioXActionResultWrapper();
        }
    }
}