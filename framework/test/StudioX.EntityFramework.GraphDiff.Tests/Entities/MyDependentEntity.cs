using StudioX.Domain.Entities;

namespace StudioX.EntityFramework.GraphDIff.Tests.Entities
{
    public class MyDependentEntity : Entity
    {
        public virtual MyMainEntity MyMainEntity { get; set; }
    }
}
