
/* see https://github.com/dotnet/corefx/issues/8681#issuecomment-290396746 */

//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using Shouldly;
//using Xunit;

//namespace StudioX.TestBase.SampleApplication.Tests.Uow
//{
//    public class AsyncLocalTests
//    {
//        private static readonly AsyncLocal<string> asyncLocal = new AsyncLocal<string>();

//        [Fact]
//        public async Task Test1()
//        {
//            asyncLocal.Value = "XX";

//            await AsyncTestCode("42");

//            asyncLocal.Value.ShouldBe("42");
//        }

//        private static async Task AsyncTestCode(string value)
//        {
//            using (var ms = new MemoryStream())
//            {
//                await ms.WriteAsync(new[] { (byte)1 }, 0, 1);

//                asyncLocal.Value = value;
//                asyncLocal.Value.ShouldBe(value);

//                await ms.WriteAsync(new[] { (byte)2 }, 0, 1);
//            }
//        }
//    }
//}
