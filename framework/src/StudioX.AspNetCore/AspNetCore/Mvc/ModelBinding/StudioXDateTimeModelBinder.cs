using System;
using System.Threading.Tasks;
using StudioX.Timing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace StudioX.AspNetCore.Mvc.ModelBinding
{
    public class StudioXDateTimeModelBinder : IModelBinder
    {
        private readonly Type type;
        private readonly SimpleTypeModelBinder simpleTypeModelBinder;

        public StudioXDateTimeModelBinder(Type type)
        {
            this.type = type;
            simpleTypeModelBinder = new SimpleTypeModelBinder(type);
        }
        
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await simpleTypeModelBinder.BindModelAsync(bindingContext);

            if (!bindingContext.Result.IsModelSet)
            {
                return;
            }

            if (type == typeof(DateTime))
            {
                var dateTime = (DateTime)bindingContext.Result.Model;
                bindingContext.Result = ModelBindingResult.Success(Clock.Normalize(dateTime));
            }
            else
            {
                var dateTime = (DateTime?)bindingContext.Result.Model;
                if (dateTime != null)
                {
                    bindingContext.Result = ModelBindingResult.Success(Clock.Normalize(dateTime.Value));
                }
            }
        }
    }
}