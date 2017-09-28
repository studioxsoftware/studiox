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
        private readonly IStudioXAspNetCoreConfiguration _configuration;
        private readonly IAuditingHelper _auditingHelper;

        public StudioXAuditActionFilter(IStudioXAspNetCoreConfiguration configuration, IAuditingHelper auditingHelper)
        {
            _configuration = configuration;
            _auditingHelper = auditingHelper;
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
                var auditInfo = _auditingHelper.CreateAuditInfo(
                    context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.AsType(),
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
                    await _auditingHelper.SaveAsync(auditInfo);
                }
            }
        }

        private bool ShouldSaveAudit(ActionExecutingContext actionContext)
        {
            return _configuration.IsAuditingEnabled &&
                   actionContext.ActionDescriptor.IsControllerAction() &&
                   _auditingHelper.ShouldSaveAudit(actionContext.ActionDescriptor.GetMethodInfo(), true);
        }
    }
}
