using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Results.Wrapping
{
    public class NullStudioXActionResultWrapper : IStudioXActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {
            
        }
    }
}