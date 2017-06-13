using System.Data.Common;
using System.Data.Entity;
using StudioX.Domain.Repositories;
using StudioX.EntityFramework;
using StudioX.TestBase.SampleApplication.ContacLists;
using StudioX.TestBase.SampleApplication.Crm;
using StudioX.TestBase.SampleApplication.EntityFramework.Repositories;
using StudioX.TestBase.SampleApplication.GuidEntities;
using StudioX.TestBase.SampleApplication.Messages;
using StudioX.TestBase.SampleApplication.People;

namespace StudioX.TestBase.SampleApplication.EntityFramework
{
    [AutoRepositoryTypes(
        typeof(IRepository<>),
        typeof(IRepository<,>),
        typeof(SampleApplicationEfRepositoryBase<>),
        typeof(SampleApplicationEfRepositoryBase<,>)
    )]
    public class SampleApplicationDbContext : StudioXDbContext
    {
        public virtual IDbSet<ContactList> ContactLists { get; set; }

        public virtual IDbSet<Person> People { get; set; }

        public virtual IDbSet<Message> Messages { get; set; }

        public virtual IDbSet<Company> Companies { get; set; }

        public virtual IDbSet<Branch> Branches { get; set; }

        public virtual IDbSet<TestEntityWithGuidPk> TestEntityWithGuidPks { get; set; }

        public virtual IDbSet<TestEntityWithGuidPkAndDbGeneratedValue> TestEntityWithGuidPkAndDbGeneratedValues { get; set; }

        public SampleApplicationDbContext()
        {

        }

        public SampleApplicationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public SampleApplicationDbContext(DbConnection connection)
            : base(connection, false)
        {

        }

        public SampleApplicationDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {

        }
    }
}
