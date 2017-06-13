using System.Collections.Generic;
using StudioX.Domain.Entities;

namespace StudioX.EntityFramework.GraphDIff.Tests.Entities
{
    public class MyMainEntity : Entity
    {
        public virtual ICollection<MyDependentEntity> MyDependentEntities { get; set; }
    }
}