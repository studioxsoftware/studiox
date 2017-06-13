using StudioX.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Results.Wrapping
{
    public class StudioXEmptyActionResultWrapper : IStudioXActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {
            actionResult.Result = new ObjectResult(new AjaxResponse());
        }
    }
}