using System.Threading.Tasks;
using StudioX.Domain.Entities;
using StudioX.TestBase;
using StudioX.Web.Models;
using Xunit;

namespace StudioX.Web.Common.Tests.Web
{
    public class DefaultErrorInfoConverterTests : StudioXIntegratedTestBase<StudioXWebCommonTestModule>
    {
        private readonly DefaultErrorInfoConverter defaultErrorInfoConverter;

        public DefaultErrorInfoConverterTests()
        {
            defaultErrorInfoConverter = Resolve<DefaultErrorInfoConverter>();
        }

        [Fact]
        public async Task DefaultErrorInfoConverterShouldWorkForEntityNotFoundExceptionOverloadMethods()
        {
            var message = "Test message";
            var errorInfo = defaultErrorInfoConverter.Convert(new EntityNotFoundException(message));

            Assert.Equal(errorInfo.Message, message);

            var exception = new EntityNotFoundException();
            errorInfo = defaultErrorInfoConverter.Convert(exception);

            Assert.Equal(errorInfo.Message, exception.Message);
        }
    }
}