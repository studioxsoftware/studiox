using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using StudioX.AspNetCore.App.Controllers;
using StudioX.Web.Models;
using Shouldly;
using Xunit;

namespace StudioX.AspNetCore.Tests
{
    public class ValidationTests : AppTestBase
    {
        [Fact]
        public async Task ShouldWorkWithValidParametersActionResult()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetContentValue),
                    new { value = 42 }
                )
            );

            response.ShouldBe("OK: 42");
        }

        [Fact]
        public async Task ShouldWorkWithValidParametersJsonResult()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<ValidationTestController.ValidationTestArgument1>>(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetJsonValue),
                    new { value = 42 }
                )
            );

            response.Success.ShouldBeTrue();
            response.Result.Value.ShouldBe(42);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData("undefined")]
        [InlineData(null)]
        public async Task ShouldNotWorkWithInvalidParameters(object value)
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<ValidationTestController.ValidationTestArgument1>>(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetJsonValue),
                    new { value = value }
                ),
                HttpStatusCode.BadRequest
            );

            response.Success.ShouldBeFalse();
            response.Result.ShouldBeNull();
            response.Error.ShouldNotBeNull();
            response.Error.ValidationErrors.ShouldNotBeNull();
            response.Error.ValidationErrors[0].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[0].Members[0].ShouldBe("value");
        }

        [Fact]
        public async Task ShouldNotWorkWithInvalidParametersNoParameterProvided()
        {
            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<ValidationTestController.ValidationTestArgument1>>(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetJsonValue)
                ),
                HttpStatusCode.BadRequest
            );

            response.Success.ShouldBeFalse();
            response.Result.ShouldBeNull();
            response.Error.ShouldNotBeNull();
            response.Error.ValidationErrors.ShouldNotBeNull();
            response.Error.ValidationErrors[0].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[0].Members[0].ShouldBe("value");
        }

        [Fact]
        public async Task ShouldNotWorkWithInvalidParametersEnum()
        {
            // Act
            var response = await PostAsync<AjaxResponse<ValidationTestController.ValidationTestArgument2>>(
                GetUrl<ValidationTestController>(
                    nameof(ValidationTestController.GetJsonValueWithEnum)
                ),
                new StringContent("{ \"value\": \"asd\" }", Encoding.UTF8, "application/json"),
                HttpStatusCode.BadRequest
            );

            response.Success.ShouldBeFalse();
            response.Result.ShouldBeNull();
            response.Error.ShouldNotBeNull();
            response.Error.ValidationErrors.Length.ShouldBe(1);
            response.Error.ValidationErrors.ShouldNotBeNull();
            response.Error.ValidationErrors[0].Members.Length.ShouldBe(1);
            response.Error.ValidationErrors[0].Members[0].ShouldBe("value");
        }
    }
}
