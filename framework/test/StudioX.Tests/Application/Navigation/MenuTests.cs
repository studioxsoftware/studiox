using System.Linq;
using System.Threading.Tasks;
using StudioX.Application.Navigation;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Application.Navigation
{
    public class MenuTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public async Task TestMenuSystem()
        {
            var testCase = new NavigationTestCase();

            //Check created menu definitions
            var mainMenuDefinition = testCase.NavigationManager.MainMenu;
            mainMenuDefinition.Items.Count.ShouldBe(1);

            var adminMenuItemDefinition = mainMenuDefinition.GetItemByNameOrNull("StudioX.Zero.Administration");
            adminMenuItemDefinition.ShouldNotBe(null);
            adminMenuItemDefinition.Items.Count.ShouldBe(3);
            
            //Check user menus
            var userMenu = await testCase.UserNavigationManager.GetMenuAsync(mainMenuDefinition.Name, new UserIdentifier(1, 1));
            userMenu.Items.Count.ShouldBe(1);

            var userAdminMenu = userMenu.Items.FirstOrDefault(i => i.Name == "StudioX.Zero.Administration");
            userAdminMenu.ShouldNotBe(null);

            userAdminMenu.Items.FirstOrDefault(i => i.Name == "StudioX.Zero.Administration.User").ShouldNotBe(null);
            userAdminMenu.Items.FirstOrDefault(i => i.Name == "StudioX.Zero.Administration.Role").ShouldBe(null);
            userAdminMenu.Items.FirstOrDefault(i => i.Name == "StudioX.Zero.Administration.Setting").ShouldNotBe(null);
        }
    }
}
