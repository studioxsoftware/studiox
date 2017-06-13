using System;
using System.Threading.Tasks;
using StudioX.AspNetCore.App.Controllers;
using StudioX.Configuration.Startup;
using StudioX.MultiTenancy;
using StudioX.Web.Models;
using StudioX.Web.MultiTenancy;
using Microsoft.Net.Http.Headers;
using Shouldly;
using Xunit;

namespace StudioX.AspNetCore.Tests
{
    public class MultiTenancyTests : AppTestBase
    {
        private readonly IWebMultiTenancyConfiguration multiTenancyConfiguration;

        public MultiTenancyTests()
        {
            IocManager.Resolve<IMultiTenancyConfig>().IsEnabled = true;
            multiTenancyConfiguration = Resolve<IWebMultiTenancyConfiguration>();
        }

        [Fact]
        public async Task HttpHeaderTenantResolveContributor()
        {
            Client.DefaultRequestHeaders.Add(MultiTenancyConsts.TenantIdResolveKey, "42");

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(42);
        }

        [Fact]
        public async Task HttpCookieTenantResolveContributor()
        {
            Client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(MultiTenancyConsts.TenantIdResolveKey, "42").ToString());

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(42);
        }

        [Fact]
        public async Task HeaderShouldHaveHighPriorityThanCookie()
        {
            Client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(MultiTenancyConsts.TenantIdResolveKey, "43").ToString());
            Client.DefaultRequestHeaders.Add(MultiTenancyConsts.TenantIdResolveKey, "42");

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(42);
        }

        [Theory]
        [InlineData("http://{TENANCYNAME}.mysite.com", "http://default.mysite.com")]
        [InlineData("http://{TENANCYNAME}.mysite.com:8080", "http://default.mysite.com:8080")]
        [InlineData("http://{TENANCYNAME}.mysite.com/", "http://default.mysite.com/")]
        public async Task DomainTenantResolveContributorTest(string domainFormat, string domain)
        {
            multiTenancyConfiguration.DomainFormat = domainFormat;
            Client.BaseAddress = new Uri(domain);

            // Act
            var response = await GetResponseAsObjectAsync<AjaxResponse<int?>>(
                GetUrl<MultiTenancyTestController>(
                    nameof(MultiTenancyTestController.GetTenantId)
                )
            );

            //Assert
            response.Result.ShouldBe(1);
        }
    }
}
