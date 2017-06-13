using System;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.MultiTenancy;
using StudioX.Runtime;
using StudioX.Runtime.Session;

namespace StudioX.TestBase.Runtime.Session
{
    public class TestStudioXSession : IStudioXSession, ISingletonDependency
    {
        public virtual long? UserId
        {
            get
            {
                if (sessionOverrideScopeProvider.GetValue(StudioXSessionBase.SessionOverrideContextKey) != null)
                {
                    return sessionOverrideScopeProvider.GetValue(StudioXSessionBase.SessionOverrideContextKey).UserId;
                }

                return userId;
            }
            set => userId = value;
        }

        public virtual int? TenantId
        {
            get
            {
                if (!multiTenancy.IsEnabled)
                {
                    return 1;
                }

                if (sessionOverrideScopeProvider.GetValue(StudioXSessionBase.SessionOverrideContextKey) != null)
                {
                    return sessionOverrideScopeProvider.GetValue(StudioXSessionBase.SessionOverrideContextKey).TenantId;
                }

                var resolvedValue = tenantResolver.ResolveTenantId();
                if (resolvedValue != null)
                {
                    return resolvedValue;
                }

                return tenantId;
            }
            set
            {
                if (!multiTenancy.IsEnabled && value != 1 && value != null)
                {
                    throw new StudioXException("Can not set TenantId since multi-tenancy is not enabled. Use IMultiTenancyConfig.IsEnabled to enable it.");
                }

                tenantId = value;
            }
        }

        public virtual MultiTenancySides MultiTenancySide { get { return GetCurrentMultiTenancySide(); } }
        
        public virtual long? ImpersonatorUserId { get; set; }
        
        public virtual int? ImpersonatorTenantId { get; set; }

        private readonly IMultiTenancyConfig multiTenancy;
        private readonly IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider;
        private readonly ITenantResolver tenantResolver;
        private int? tenantId;
        private long? userId;

        public TestStudioXSession(
            IMultiTenancyConfig multiTenancy, 
            IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider,
            ITenantResolver tenantResolver)
        {
            this.multiTenancy = multiTenancy;
            this.sessionOverrideScopeProvider = sessionOverrideScopeProvider;
            this.tenantResolver = tenantResolver;
        }

        protected virtual MultiTenancySides GetCurrentMultiTenancySide()
        {
            return multiTenancy.IsEnabled && !TenantId.HasValue
                ? MultiTenancySides.Host
                : MultiTenancySides.Tenant;
        }

        public virtual IDisposable Use(int? tenantId, long? userId)
        {
            return sessionOverrideScopeProvider.BeginScope(StudioXSessionBase.SessionOverrideContextKey, new SessionOverride(tenantId, userId));
        }
    }
}