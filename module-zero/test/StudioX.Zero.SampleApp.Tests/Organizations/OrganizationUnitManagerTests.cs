using System.Linq;
using System.Threading.Tasks;
using StudioX.Organizations;
using StudioX.UI;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Organizations
{
    public class OrganizationUnitManagerTests : SampleAppTestBase
    {
        private readonly OrganizationUnitManager organizationUnitManager;

        public OrganizationUnitManagerTests()
        {
            StudioXSession.TenantId = GetDefaultTenant().Id;

            organizationUnitManager = Resolve<OrganizationUnitManager>();
        }

        [Fact]
        public async Task ShouldCreateRootOU()
        {
            //Act
            await organizationUnitManager.CreateAsync(new OrganizationUnit(StudioXSession.TenantId, "Root 1"));

            //Assert
            var root1 = GetOUOrNull("Root 1");
            root1.ShouldNotBeNull();
            root1.Code.ShouldBe(OrganizationUnit.CreateCode(3));
        }

        [Fact]
        public async Task ShouldCreateChildOU()
        {
            //Arrange
            var ou11 = GetOU("OU11");

            //Act
            await organizationUnitManager.CreateAsync(new OrganizationUnit(StudioXSession.TenantId, "OU11 New Child", ou11.Id));

            //Assert
            var newChild = GetOUOrNull("OU11 New Child");
            newChild.ShouldNotBeNull();
            newChild.ParentId.ShouldBe(ou11.Id);
            newChild.Code.ShouldBe(OrganizationUnit.CreateCode(1, 1, 3));
        }
        
        [Fact]
        public async Task ShouldNotCreateOUWithSameNameInSameLevel()
        {
            //Arrange
            var ou11 = GetOU("OU11");

            //Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(
                () => organizationUnitManager.CreateAsync(
                    new OrganizationUnit(StudioXSession.TenantId, "OU112", ou11.Id)
                    )
                );
        }

        [Fact]
        public async Task ShouldDeleteUOWithChildren()
        {
            //Arrange
            var ou11 = GetOU("OU11");

            //Act
            await organizationUnitManager.DeleteAsync(ou11.Id);

            //Assert
            GetOU("OU11").IsDeleted.ShouldBeTrue();
            GetOU("OU111").IsDeleted.ShouldBeTrue();
            GetOU("OU112").IsDeleted.ShouldBeTrue();
        }

        /* Moves UO1 under OU2 (with all children) */
        [Fact]
        public async Task ShouldMoveOUUnderOtherOUWithChildren()
        {
            //Arrange
            var ou1 = GetOU("OU1");
            var ou2 = GetOU("OU2");

            //Act
            await organizationUnitManager.MoveAsync(ou1.Id, ou2.Id);

            //Assert
            ou1 = GetOU("OU1");
            ou1.ParentId.ShouldBe(ou2.Id);
            ou1.Code.ShouldBe(OrganizationUnit.CreateCode(2, 2));

            var ou11 = GetOU("OU11");
            ou11.ParentId.ShouldBe(ou1.Id);
            ou11.Code.ShouldBe(OrganizationUnit.CreateCode(2, 2, 1));

            var ou111 = GetOU("OU111");
            ou111.ParentId.ShouldBe(ou11.Id);
            ou111.Code.ShouldBe(OrganizationUnit.CreateCode(2, 2, 1, 1));

            var ou112 = GetOU("OU112");
            ou112.ParentId.ShouldBe(ou11.Id);
            ou112.Code.ShouldBe(OrganizationUnit.CreateCode(2, 2, 1, 2));

            var ou12 = GetOU("OU12");
            ou12.ParentId.ShouldBe(ou1.Id);
            ou12.Code.ShouldBe(OrganizationUnit.CreateCode(2, 2, 2));
        }

        /* Moves UO11 to ROOT (with all children) */
        [Fact]
        public async Task ShouldMoveOUToRootWithChildren()
        {
            //Arrange
            var ou11 = GetOU("OU11");

            //Act
            await organizationUnitManager.MoveAsync(ou11.Id, null);

            //Assert
            ou11 = GetOU("OU11");
            ou11.ParentId.ShouldBe(null);
            ou11.Code.ShouldBe(OrganizationUnit.CreateCode(3));

            var ou111 = GetOU("OU111");
            ou111.ParentId.ShouldBe(ou11.Id);
            ou111.Code.ShouldBe(OrganizationUnit.CreateCode(3, 1));

            var ou112 = GetOU("OU112");
            ou112.ParentId.ShouldBe(ou11.Id);
            ou112.Code.ShouldBe(OrganizationUnit.CreateCode(3, 2));
        }

        private OrganizationUnit GetOU(string diplayName)
        {
            var organizationUnit = UsingDbContext(context => context.OrganizationUnits.FirstOrDefault(ou => ou.DisplayName == diplayName));
            organizationUnit.ShouldNotBeNull();
            return organizationUnit;
        }

        private OrganizationUnit GetOUOrNull(string diplayName)
        {
            return UsingDbContext(context => context.OrganizationUnits.FirstOrDefault(ou => ou.DisplayName == diplayName));
        }
    }
}
