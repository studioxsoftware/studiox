using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StudioX.Aspects;
using StudioX.Threading;
using Castle.DynamicProxy;

namespace StudioX.Auditing
{
    internal class AuditingInterceptor : IInterceptor
    {
        private readonly IAuditingHelper auditingHelper;

        public AuditingInterceptor(IAuditingHelper auditingHelper)
        {
            this.auditingHelper = auditingHelper;
        }

        public void Intercept(IInvocation invocation)
        {
            if (StudioXCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, StudioXCrossCuttingConcerns.Auditing))
            {
                invocation.Proceed();
                return;
            }

            if (!auditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }

            var auditInfo = auditingHelper.CreateAuditInfo(invocation.MethodInvocationTarget, invocation.Arguments);

            if (AsyncHelper.IsAsyncMethod(invocation.Method))
            {
                PerformAsyncAuditing(invocation, auditInfo);
            }
            else
            {
                PerformSyncAuditing(invocation, auditInfo);
            }
        }

        private void PerformSyncAuditing(IInvocation invocation, AuditInfo auditInfo)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                auditingHelper.Save(auditInfo);
            }
        }

        private void PerformAsyncAuditing(IInvocation invocation, AuditInfo auditInfo)
        {
            var stopwatch = Stopwatch.StartNew();

            invocation.Proceed();

            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithFinally(
                    (Task) invocation.ReturnValue,
                    exception => SaveAuditInfo(auditInfo, stopwatch, exception)
                    );
            }
            else //Task<TResult>
            {
                invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithFinallyAndGetResult(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    invocation.ReturnValue,
                    exception => SaveAuditInfo(auditInfo, stopwatch, exception)
                    );
            }
        }

        private void SaveAuditInfo(AuditInfo auditInfo, Stopwatch stopwatch, Exception exception)
        {
            stopwatch.Stop();
            auditInfo.Exception = exception;
            auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

            auditingHelper.Save(auditInfo);
        }
    }
}