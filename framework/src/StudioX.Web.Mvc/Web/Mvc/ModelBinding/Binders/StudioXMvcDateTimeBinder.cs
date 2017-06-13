using System;
using System.Web.Mvc;
using StudioX.Timing;

namespace StudioX.Web.Mvc.ModelBinding.Binders
{
    /// <summary>
    /// Binds any browser request datetime to utc datetime
    /// </summary>
    public class StudioXMvcDateTimeBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var date = base.BindModel(controllerContext, bindingContext) as DateTime?;
            if (date.HasValue)
            {
                return Clock.Normalize(date.Value);
            }

            return null;
        }
    }
}
