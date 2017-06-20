using StudioX.Aspects;
using StudioX.Dependency;
using Castle.DynamicProxy;

namespace StudioX.Runtime.Validation.Interception
{
    /// <summary>
    /// This interceptor is used intercept method calls for classes which's methods must be validated.
    /// </summary>
    public class ValidationInterceptor : IInterceptor
    {
        private readonly IIocResolver iocResolver;

        public ValidationInterceptor(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
        }

        public void Intercept(IInvocation invocation)
        {
            if (StudioXCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, StudioXCrossCuttingConcerns.Validation))
            {
                invocation.Proceed();
                return;
            }

            using (var validator = iocResolver.ResolveAsDisposable<MethodInvocationValidator>())
            {
                validator.Object.Initialize(invocation.MethodInvocationTarget, invocation.Arguments);
                validator.Object.Validate();
            }
            
            invocation.Proceed();
        }
    }
}
