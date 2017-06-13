using System;
using StudioX.Configuration.Startup;
using StudioX.Dependency.Installers;
using StudioX.Localization;
using StudioX.Tests;
using StudioX.UI;
using StudioX.Web.Configuration;
using StudioX.Web.Models;
using NSubstitute;
using Xunit;

namespace StudioX.Web.Tests
{
    public class ErrorInfoBuilderTests : TestBaseWithLocalIocManager
    {
        private readonly IErrorInfoBuilder errorInfoBuilder;

        public ErrorInfoBuilderTests()
        {
            LocalIocManager.IocContainer.Install(new StudioXCoreInstaller());

            var configuration = LocalIocManager.Resolve<StudioXStartupConfiguration>();
            configuration.Initialize();
            configuration.Localization.IsEnabled = false;

            errorInfoBuilder = new ErrorInfoBuilder(Substitute.For<IStudioXWebCommonModuleConfiguration>(), NullLocalizationManager.Instance);
            errorInfoBuilder.AddExceptionConverter(new MyErrorInfoConverter());
        }

        [Fact]
        public void ShouldConvertSpecificException()
        {
            var errorInfo = errorInfoBuilder.BuildForException(new MySpecificException());
            Assert.Equal(42, errorInfo.Code);
            Assert.Equal("MySpecificMessage", errorInfo.Message);
            Assert.Equal("MySpecificMessageDetails", errorInfo.Details);
        }

        [Fact]
        public void ShouldConvertUserFriendlyException()
        {
            var errorInfo = errorInfoBuilder.BuildForException(new UserFriendlyException("Test message"));
            Assert.Equal(0, errorInfo.Code);
            Assert.Equal("Test message", errorInfo.Message);
        }

        //[Fact]
        //public void ShouldNotConvertOtherExceptions()
        //{
        //    var errorInfo = errorInfoBuilder.BuildForException(new Exception("Test message"));
        //    Assert.Equal(0, errorInfo.Code);
        //    Assert.NotEqual("Test message", errorInfo.Message);
        //}

        public class MySpecificException : Exception
        {
            
        }

        public class MyErrorInfoConverter : IExceptionToErrorInfoConverter
        {
            public IExceptionToErrorInfoConverter Next { set; private get; }

            public ErrorInfo Convert(Exception exception)
            {
                if (exception is MySpecificException)
                {
                    return new ErrorInfo(42, "MySpecificMessage", "MySpecificMessageDetails");
                }

                return Next.Convert(exception);
            }
        }
    }
}
