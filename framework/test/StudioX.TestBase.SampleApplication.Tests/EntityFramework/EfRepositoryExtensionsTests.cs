using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.EntityFramework.Repositories;
using StudioX.TestBase.SampleApplication.Crm;
using StudioX.TestBase.SampleApplication.EntityFramework;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.EntityFramework
{
    public class EfRepositoryExtensionsTests : SampleApplicationTestBase
    {
        private readonly IRepository<Company> companyRepository;

        public EfRepositoryExtensionsTests()
        {
            companyRepository = Resolve<IRepository<Company>>();
        }

        [Fact]
        public void ShouldGetDbContext()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                companyRepository.GetDbContext().ShouldBeOfType<SampleApplicationDbContext>();

                uow.Complete();
            }
        }
        
        [Fact]
        public void ShouldGetIocResolver()
        {
            companyRepository.GetIocResolver().ShouldNotBeNull();
        }
    }
}