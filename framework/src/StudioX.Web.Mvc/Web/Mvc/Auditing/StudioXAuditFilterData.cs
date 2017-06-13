using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using StudioX.Auditing;

namespace StudioX.Web.Mvc.Auditing
{
    public class StudioXAuditFilterData
    {
        private const string StudioXAuditFilterDataHttpContextKey = "__StudioXAuditFilterData";

        public Stopwatch Stopwatch { get; }

        public AuditInfo AuditInfo { get; }

        public StudioXAuditFilterData(
            Stopwatch stopwatch,
            AuditInfo auditInfo)
        {
            Stopwatch = stopwatch;
            AuditInfo = auditInfo;
        }

        public static void Set(HttpContextBase httpContext, StudioXAuditFilterData auditFilterData)
        {
            GetAuditDataStack(httpContext).Push(auditFilterData);
        }

        public static StudioXAuditFilterData GetOrNull(HttpContextBase httpContext)
        {
            var stack = GetAuditDataStack(httpContext);
            return stack.Count <= 0
                ? null
                : stack.Pop();
        }

        private static Stack<StudioXAuditFilterData> GetAuditDataStack(HttpContextBase httpContext)
        {
            var stack = httpContext.Items[StudioXAuditFilterDataHttpContextKey] as Stack<StudioXAuditFilterData>;

            if (stack == null)
            {
                stack = new Stack<StudioXAuditFilterData>();
                httpContext.Items[StudioXAuditFilterDataHttpContextKey] = stack;
            }

            return stack;
        }
    }
}