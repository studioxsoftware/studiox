using System;
using StudioX.Configuration.Startup;
using StudioX.MultiTenancy;

namespace StudioX.Runtime.Session
{
    public abstract class StudioXSessionBase : IStudioXSession
    {
        public const string SessionOverrideContextKey = "StudioX.Runtime.Session.Override";

        public IMultiTenancyConfig MultiTenancy { get; }

        public abstract long? UserId { get; }

        public abstract int? TenantId { get; }

        public abstract long? ImpersonatorUserId { get; }

        public abstract int? ImpersonatorTenantId { get; }

        public virtual MultiTenancySides MultiTenancySide
        {
            get
            {
                return MultiTenancy.IsEnabled && !TenantId.HasValue
                    ? MultiTenancySides.Host
                    : MultiTenancySides.Tenant;
            }
        }

        protected SessionOverride OverridedValue => SessionOverrideScopeProvider.GetValue(SessionOverrideContextKey);
        protected IAmbientScopeProvider<SessionOverride> SessionOverrideScopeProvider { get; }

        protected StudioXSessionBase(IMultiTenancyConfig multiTenancy, IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider)
        {
            MultiTenancy = multiTenancy;
            SessionOverrideScopeProvider = sessionOverrideScopeProvider;
        }

        public IDisposable Use(int? tenantId, long? userId)
        {
            return SessionOverrideScopeProvider.BeginScope(SessionOverrideContextKey, new SessionOverride(tenantId, userId));
        }
    }
}