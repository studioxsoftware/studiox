using System;
using System.Diagnostics;
using System.Threading.Tasks;
using StudioX.Aspects;
using StudioX.AspNetCore.Configuration;
using StudioX.AspNetCore.Mvc.Extensions;
using StudioX.Auditing;
using StudioX.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudioX.AspNetCore.Mvc.Auditing
{
    public class StudioXAuditActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IStudioXAspNetCoreConfiguration configuration;
        private readonly IAuditingHelper auditingHelper;

        public StudioXAuditActionFilter(IStudioXAspNetCoreConfiguration configuration, IAuditingHelper auditingHelper)
        {
            this.configuration = configuration;
            this.auditingHelper = auditingHelper;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!ShouldSaveAudit(context))
            {
                await next();
                return;
            }

            using (StudioXCrossCuttingConcerns.Applying(context.Controller, StudioXCrossCuttingConcerns.Auditing))
            {
                var auditInfo = auditingHelper.CreateAuditInfo(
                    context.ActionDescriptor.AsControllerActionDescriptor().MethodInfo,
                    context.ActionArguments
                );

                var stopwatch = Stopwatch.StartNew();

                try
                {
                    var result = await next();
                    if (result.Exception != null && !result.ExceptionHandled)
                    {
                        auditInfo.Exception = result.Exception;
                    }
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
                    await auditingHelper.SaveAsync(auditInfo);
                }
            }
        }

        private bool ShouldSaveAudit(ActionExecutingContext actionContext)
        {
            return configuration.IsAuditingEnabled &&
                   auditingHelper.ShouldSaveAudit(actionContext.ActionDescriptor.GetMethodInfo(), true);
        }
    }
}
