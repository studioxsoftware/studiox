using System;
using System.ComponentModel.DataAnnotations.Schema;
using StudioX.Domain.Entities;
using StudioX.Domain.Entities.Auditing;

namespace StudioX.TestBase.SampleApplication.Crm
{
    [Table("Branches")]
    public class Branch : Entity, IHasCreationTime
    {
        public int CompanyId { get; set; }

        public string Name { get; set; }

        public DateTime CreationTime { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }
    }
}