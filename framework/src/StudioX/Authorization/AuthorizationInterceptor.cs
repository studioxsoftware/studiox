using Castle.DynamicProxy;

namespace StudioX.Authorization
{
    /// <summary>
    ///     This class is used to intercept methods to make authorization if the method defined
    ///     <see cref="StudioXAuthorizeAttribute" />.
    /// </summary>
    public class AuthorizationInterceptor : IInterceptor
    {
        private readonly IAuthorizationHelper authorizationHelper;

        public AuthorizationInterceptor(IAuthorizationHelper authorizationHelper)
        {
            this.authorizationHelper = authorizationHelper;
        }

        public void Intercept(IInvocation invocation)
        {
            authorizationHelper.Authorize(invocation.MethodInvocationTarget, invocation.TargetType);
            invocation.Proceed();
        }
    }
}