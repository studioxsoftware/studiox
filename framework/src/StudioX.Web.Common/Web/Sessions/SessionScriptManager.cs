using System.Text;
using StudioX.Dependency;
using StudioX.Runtime.Session;

namespace StudioX.Web.Sessions
{
    public class SessionScriptManager : ISessionScriptManager, ITransientDependency
    {
        public IStudioXSession StudioXSession { get; set; }

        public SessionScriptManager()
        {
            StudioXSession = NullStudioXSession.Instance;
        }

        public string GetScript()
        {
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            script.AppendLine();

            script.AppendLine("    studiox.session = studiox.session || {};");
            script.AppendLine("    studiox.session.userId = " + (StudioXSession.UserId.HasValue ? StudioXSession.UserId.Value.ToString() : "null") + ";");
            script.AppendLine("    studiox.session.tenantId = " + (StudioXSession.TenantId.HasValue ? StudioXSession.TenantId.Value.ToString() : "null") + ";");
            script.AppendLine("    studiox.session.impersonatorUserId = " + (StudioXSession.ImpersonatorUserId.HasValue ? StudioXSession.ImpersonatorUserId.Value.ToString() : "null") + ";");
            script.AppendLine("    studiox.session.impersonatorTenantId = " + (StudioXSession.ImpersonatorTenantId.HasValue ? StudioXSession.ImpersonatorTenantId.Value.ToString() : "null") + ";");
            script.AppendLine("    studiox.session.multiTenancySide = " + ((int)StudioXSession.MultiTenancySide) + ";");

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }
    }
}