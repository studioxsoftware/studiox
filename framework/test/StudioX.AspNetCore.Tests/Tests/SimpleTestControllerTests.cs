using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StudioX.AspNetCore.App.Controllers;
using StudioX.AspNetCore.App.Models;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Exceptions;
using StudioX.UI;
using StudioX.Web.Models;
using Microsoft.AspNetCore.Localization;
using Shouldly;
using Xunit;

namespace StudioX.AspNetCore.Tests
{
    public class SimpleTestControllerTests : AppTestBase
    {
        [Fact]
        public void ShouldResolveController()
        {
            ServiceProvider.GetService<SimpleTestController>().ShouldNotBeNull();
        }

        [Fact]
        public async Task ShouldReturnContent()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                               GetUrl<SimpleTestController>(
                                   nameof(SimpleTestController.SimpleContent)
                               )
                           );

            // Assert
            response.ShouldBe("Hello world...");
        }

        [Fact]
        public async Task ShouldWrapJsonByDefault()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<SimpleViewModel>>(
                               GetUrl<SimpleTestController>(
                                   nameof(SimpleTestController.SimpleJson)
                               )
                           );

            //Assert
            response.Result.StrValue.ShouldBe("Forty Two");
            response.Result.IntValue.ShouldBe(42);
        }

        [Theory]
        [InlineData(true, "This is a user friendly exception message")]
        [InlineData(false, "This is an exception message")]
        public async Task ShouldWrapJsonExceptionByDefault(bool userFriendly, string message)
        {
            //Arrange

            var exceptionEventRaised = false;
            Resolve<IEventBus>().Register<StudioXHandledExceptionData>(data =>
            {
                exceptionEventRaised = true;
                data.Exception.ShouldNotBeNull();
                data.Exception.Message.ShouldBe(message);
            });

            // Act

            var response = await GetResponseAsObjectAsync<AjaxResponse<SimpleViewModel>>(
                               GetUrl<SimpleTestController>(
                                   nameof(SimpleTestController.SimpleJsonException),
                                   new
                                   {
                                       message,
                                       userFriendly
                                   }),
                               HttpStatusCode.InternalServerError
                           );

            //Assert

            response.Error.ShouldNotBeNull();
            if (userFriendly)
            {
                response.Error.Message.ShouldBe(message);
            }
            else
            {
                response.Error.Message.ShouldNotBe(message);
            }

            exceptionEventRaised.ShouldBeTrue();
        }

        [Fact]
        public async Task ShouldNotWrapJsonExceptionIfRequested()
        {
            //Act & Assert
            await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await GetResponseAsObjectAsync<AjaxResponse<SimpleViewModel>>(
                    GetUrl<SimpleTestController>(
                        nameof(SimpleTestController.SimpleJsonExceptionDownWrap)
                    ));
            });
        }

        [Fact]
        public async Task ShouldNotWrapJsonIfDontWrapDeclared()
        {
            // Act
            var response = await GetResponseAsObjectAsync<SimpleViewModel>(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.SimpleJsonDontWrap)
                ));

            //Assert
            response.StrValue.ShouldBe("Forty Two");
            response.IntValue.ShouldBe(42);
        }

        [Fact]
        public async Task ShouldWrapVoidMethods()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse>(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.GetVoidTest)
                ));

            response.Success.ShouldBeTrue();
            response.Result.ShouldBeNull();
        }

        [Fact]
        public async Task ShouldNotWrapVoidMethodsIfRequested()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.GetVoidTestDontWrap)
                ));

            response.ShouldBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldNotWrapActionResult()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.GetActionResultTest)
                ));

            //Assert
            response.ShouldBe("GetActionResultTest-Result");
        }

        [Fact]
        public async Task ShouldNotWrapAsyncActionResult()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.GetActionResultTestAsync)
                ));

            //Assert
            response.ShouldBe("GetActionResultTestAsync-Result");
        }

        [Fact]
        public async Task ShouldWrapAsyncVoidOnException()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse>(
                GetUrl<SimpleTestController>(
                    nameof(SimpleTestController.GetVoidExceptionTestAsync)
                ), HttpStatusCode.InternalServerError);

            response.Error.ShouldNotBeNull();
            response.Error.Message.ShouldBe("GetVoidExceptionTestAsync-Exception");
            response.Result.ShouldBeNull();
        }

        [Fact]
        public async Task ShouldNotWrapAsyncActionResultOnException()
        {
            // Act
            (await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await GetResponseAsStringAsync(
                    GetUrl<SimpleTestController>(
                        nameof(SimpleTestController.GetActionResultExceptionTestAsync)
                    ), HttpStatusCode.InternalServerError);
            })).Message.ShouldBe("GetActionResultExceptionTestAsync-Exception");
        }

        [Fact]
        public async Task StudioXLocalizationHeaderRequestCultureProviderTest()
        {
            //Arrange
            Client.DefaultRequestHeaders.Add(CookieRequestCultureProvider.DefaultCookieName, "c=it|uic=it");

            var culture = await GetResponseAsStringAsync(
                    GetUrl<SimpleTestController>(
                        nameof(SimpleTestController.GetCurrentCultureNameTest)
                    ));

            culture.ShouldBe("it");
        }

    }
}