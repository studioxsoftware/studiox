using System.Threading.Tasks;
using StudioX.AspNetCore.App.Controllers;
using StudioX.Web.Models;
using Shouldly;
using Xunit;

namespace StudioX.AspNetCore.Tests
{
    public class NameConflictTests : AppTestBase
    {
        [Fact]
        public async Task UrlActionShouldReturnControllerPathByDefault()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                GetUrl<NameConflictController>(
                    nameof(NameConflictController.GetSelfActionUrl)
                )
            );

            //Assert
            response.Result.ShouldBe("/NameConflict/GetSelfActionUrl");
        }

        [Fact]
        public async Task UrlActionShouldReturnControllerPathWithAreaName()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                GetUrl<NameConflictController>(
                    nameof(NameConflictController.GetAppServiceActionUrlWithArea)
                )
            );

            //Assert
            response.Result.ShouldBe("/api/services/app/NameConflict/GetConstantString");
        }

        [Fact]
        public async Task ShouldUseAppServiceWithFullRoute()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<string>>(
                "/api/services/app/NameConflict/GetConstantString"
            );

            //Assert
            response.Result.ShouldBe("return-value-from-app-service");
        }
    }
}