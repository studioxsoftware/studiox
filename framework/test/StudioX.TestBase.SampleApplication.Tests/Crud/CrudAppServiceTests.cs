using StudioX.Application.Services.Dto;
using StudioX.Authorization;
using StudioX.TestBase.SampleApplication.Crm;
using NSubstitute;
using NSubstitute.Extensions;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.SampleApplication.Tests.Crud
{
    public class CrudAppServiceTests : SampleApplicationTestBase
    {
        private readonly CompanyAppService companyAppService;
        private readonly AsyncCompanyAppService asyncCompanyAppService;


        public CrudAppServiceTests()
        {
            companyAppService = Resolve<CompanyAppService>();
            asyncCompanyAppService = Resolve<AsyncCompanyAppService>();
        }

        [Fact]
        public void ShouldNotGetAllCompaniesIfNotAuthorized()
        {
            //Arrange
            companyAppService.PermissionChecker = GetBlockerPermissionsChecker();

            //Act

            Should.Throw<StudioXAuthorizationException>(() =>
            {
                companyAppService.GetAll(new PagedAndSortedResultRequestDto());
            });
        }

        [Fact]
        public void ShouldGetAllCompaniesIfAuthorized()
        {
            companyAppService.GetAll(new PagedAndSortedResultRequestDto()).TotalCount.ShouldBe(2);
        }

        [Fact]
        public void ShouldNotDeleteCompanyIfNotAuthorized()
        {
            //Arrange
            asyncCompanyAppService.PermissionChecker = GetBlockerPermissionsChecker();

            //Act

            Should.Throw<StudioXAuthorizationException>(async () =>
            {
                await asyncCompanyAppService.Delete(new EntityDto(1));
            });
        }

        [Fact]
        public async void ShouldDeleteCompanyIfAuthorized()
        {
            //Act

            await asyncCompanyAppService.Delete(new EntityDto(1));
            (await asyncCompanyAppService.GetAll(new PagedAndSortedResultRequestDto())).TotalCount.ShouldBe(1);
        }

        private IPermissionChecker GetBlockerPermissionsChecker()
        {
            var blockerPermissionChecker = Substitute.For<IPermissionChecker>();
            blockerPermissionChecker.ReturnsForAll(false);
            return blockerPermissionChecker;
        }
    }
}
