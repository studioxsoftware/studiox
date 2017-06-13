using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using StudioX.Domain.Entities.Auditing;
using StudioX.MultiTenancy;

namespace StudioX.TestBase.SampleApplication.Crm
{
    [Table("Companies")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class Company : AuditedEntity
    {
        public string Name { get; set; }

        public virtual Address ShippingAddress { get; set; }

        public virtual Address BillingAddress { get; set; }

        [ForeignKey("CompanyId")]
        public virtual ICollection<Branch> Branches { get; set; }

        public Company()
        {
            
        }
    }
}
