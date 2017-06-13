using StudioX.Domain.Uow;
using StudioX.Web.Models;

namespace StudioX.Web.Mvc.Configuration
{
    public class StudioXMvcConfiguration : IStudioXMvcConfiguration
    {
        public UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        public WrapResultAttribute DefaultWrapResultAttribute { get; }

        public bool IsValidationEnabledForControllers { get; set; }

        public bool IsAutomaticAntiForgeryValidationEnabled { get; set; }

        public bool IsAuditingEnabled { get; set; }

        public bool IsAuditingEnabledForChildActions { get; set; }

        public StudioXMvcConfiguration()
        {
            DefaultUnitOfWorkAttribute = new UnitOfWorkAttribute();
            DefaultWrapResultAttribute = new WrapResultAttribute();
            IsValidationEnabledForControllers = true;
            IsAutomaticAntiForgeryValidationEnabled = true;
            IsAuditingEnabled = true;
        }
    }
}