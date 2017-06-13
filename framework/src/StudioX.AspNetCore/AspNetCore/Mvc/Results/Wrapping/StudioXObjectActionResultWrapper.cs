using System;
using StudioX.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Results.Wrapping
{
    public class StudioXObjectActionResultWrapper : IStudioXActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {
            var objectResult = actionResult.Result as ObjectResult;
            if (objectResult == null)
            {
                throw new ArgumentException($"{nameof(actionResult)} should be ObjectResult!");
            }

            if (!(objectResult.Value is AjaxResponseBase))
            {
                objectResult.Value = new AjaxResponse(objectResult.Value);
            }
        }
    }
}