using System.ComponentModel.DataAnnotations;
using StudioX.AspNetCore.Mvc.Extensions;
using StudioX.Collections.Extensions;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Runtime.Validation.Interception;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Validation
{
    public class MvcActionInvocationValidator : MethodInvocationValidator
    {
        protected ActionExecutingContext ActionContext { get; private set; }

        private bool isValidatedBefore;

        public MvcActionInvocationValidator(IValidationConfiguration configuration, IIocResolver iocResolver)
            : base(configuration, iocResolver)
        {

        }

        public void Initialize(ActionExecutingContext actionContext)
        {
            ActionContext = actionContext;

            SetDataAnnotationAttributeErrors();

            base.Initialize(
                actionContext.ActionDescriptor.GetMethodInfo(),
                GetParameterValues(actionContext)
            );
        }
        
        protected override void SetDataAnnotationAttributeErrors(object validatingObject)
        {
            SetDataAnnotationAttributeErrors();
        }

        protected virtual void SetDataAnnotationAttributeErrors()
        {
            if (isValidatedBefore || ActionContext.ModelState.IsValid)
            {
                return;
            }

            foreach (var state in ActionContext.ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    ValidationErrors.Add(new ValidationResult(error.ErrorMessage, new[] { state.Key }));
                }
            }

            isValidatedBefore = true;
        }

        protected virtual object[] GetParameterValues(ActionExecutingContext actionContext)
        {
            var methodInfo = actionContext.ActionDescriptor.GetMethodInfo();

            var parameters = methodInfo.GetParameters();
            var parameterValues = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                parameterValues[i] = actionContext.ActionArguments.GetOrDefault(parameters[i].Name);
            }

            return parameterValues;
        }
    }
}