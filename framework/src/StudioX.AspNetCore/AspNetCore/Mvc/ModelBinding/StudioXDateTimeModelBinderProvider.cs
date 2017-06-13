using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StudioX.AspNetCore.Mvc.ModelBinding
{
    public class StudioXDateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
            {
                return new StudioXDateTimeModelBinder(context.Metadata.ModelType);
            }

            return null;
        }
    }
}