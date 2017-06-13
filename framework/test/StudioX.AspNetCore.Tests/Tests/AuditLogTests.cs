using System.Net;
using System.Threading.Tasks;
using StudioX.AspNetCore.App.Controllers;
using StudioX.AspNetCore.App.Models;
using StudioX.AspNetCore.Mocks;
using StudioX.Web.Models;
using Shouldly;
using Xunit;

namespace StudioX.AspNetCore.Tests
{
    public class AuditLogTests : AppTestBase
    {
        private readonly MockAuditingStore mockAuditingStore;

        public AuditLogTests()
        {
            mockAuditingStore = Resolve<MockAuditingStore>();
        }

        [Fact]
        public async Task ShouldWriteAuditLogs()
        {
            mockAuditingStore.Logs.Count.ShouldBe(0);

            //Act

            await GetResponseAsObjectAsync<AjaxResponse<SimpleViewModel>>(
                   GetUrl<SimpleTestController>(
                       nameof(SimpleTestController.SimpleJsonException),
                       new
                       {
                           message = "A test message",
                           userFriendly = true
                       }),
                   HttpStatusCode.InternalServerError
               );

            //Assert

            mockAuditingStore.Logs.Count.ShouldBe(1);
            var auditLog = mockAuditingStore.Logs.ToArray()[0];
            auditLog.MethodName.ShouldBe(nameof(SimpleTestController.SimpleJsonException));
        }
    }
}