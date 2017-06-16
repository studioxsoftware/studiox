using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Results.Wrapping
{
    public class StudioXActionResultWrapperFactory : IStudioXActionResultWrapperFactory
    {
        public IStudioXActionResultWrapper CreateFor(ResultExecutingContext actionResult)
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