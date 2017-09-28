using System;
using System.Diagnostics;
using System.Web.Mvc;
using StudioX.Auditing;
using StudioX.Dependency;
using StudioX.Web.Mvc.Configuration;
using StudioX.Web.Mvc.Extensions;

namespace StudioX.Web.Mvc.Auditing
{
    public class StudioXMvcAuditFilter : IActionFilter, ITransientDependency
    {
        private readonly IStudioXMvcConfiguration _configuration;
        private readonly IAuditingHelper _auditingHelper;

        public StudioXMvcAuditFilter(IStudioXMvcConfiguration configuration, IAuditingHelper auditingHelper)
        {
            _configuration = configuration;
            _auditingHelper = auditingHelper;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!ShouldSaveAudit(filterContext))
            {
                StudioXAuditFilterData.Set(filterContext.HttpContext, null);
                return;
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerType,
                filterContext.ActionDescriptor.GetMethodInfoOrNull(),
                filterContext.ActionParameters
            );

            var actionStopwatch = Stopwatch.StartNew();

            StudioXAuditFilterData.Set(
                filterContext.HttpContext,
                new StudioXAuditFilterData(
                    actionStopwatch,
                    auditInfo
                )
            );
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var auditData = StudioXAuditFilterData.GetOrNull(filterContext.HttpContext);
            if (auditData == null)
            {
                return;
            }

            auditData.Stopwatch.Stop();

            auditData.AuditInfo.ExecutionDuration = Convert.ToInt32(auditData.Stopwatch.Elapsed.TotalMilliseconds);
            auditData.AuditInfo.Exception = filterContext.Exception;

            _auditingHelper.Save(auditData.AuditInfo);
        }

        private bool ShouldSaveAudit(ActionExecutingContext filterContext)
        {
            var currentMethodInfo = filterContext.ActionDescriptor.GetMethodInfoOrNull();
            if (currentMethodInfo == null)
            {
                return false;
            }

            if (_configuration == null)
            {
                return false;
            }

            if (!_configuration.IsAuditingEnabled)
            {
                return false;
            }

            if (filterContext.IsChildAction && !_configuration.IsAuditingEnabledForChildActions)
            {
                return false;
            }

            return _auditingHelper.ShouldSaveAudit(currentMethodInfo, true);
        }
    }
}
