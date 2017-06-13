using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using StudioX.Domain.Entities;
using StudioX.TestBase.SampleApplication.People;

namespace StudioX.TestBase.SampleApplication.ContacLists
{
    [Table("ContactLists")]
    public class ContactList : Entity, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }

        public virtual string Name { get; set; }

        [ForeignKey("ContactListId")]
        public virtual ICollection<Person> People { get; set; }
    }
}
