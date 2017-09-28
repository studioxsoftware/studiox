using StudioX.Organizations;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Organizations
{
    public class OrganizationUnitTests
    {
        [Fact]
        public void TestCreateCode()
        {
            OrganizationUnit.CreateCode().ShouldBe(null);
            OrganizationUnit.CreateCode(42).ShouldBe("00042");
            OrganizationUnit.CreateCode(1, 1, 3).ShouldBe("00001.00001.00003");
        }

        [Fact]
        public void TestAppendCode()
        {
            OrganizationUnit.AppendCode(null, "00005").ShouldBe("00005");
            OrganizationUnit.AppendCode("00042", "00034").ShouldBe("00042.00034");
        }

        [Fact]
        public void TestGetRelativeCode()
        {
            OrganizationUnit.GetRelativeCode("00042", null).ShouldBe("00042");
            OrganizationUnit.GetRelativeCode("00019.00055.00001", "00019").ShouldBe("00055.00001");
        }

        [Fact]
        public void TestCalculateNextCode()
        {
            OrganizationUnit.CalculateNextCode("00019.00055.00001").ShouldBe("00019.00055.00002");
            OrganizationUnit.CalculateNextCode("00009").ShouldBe("00010");
        }

        [Fact]
        public void TestGetLastUnitCode()
        {
            OrganizationUnit.GetLastUnitCode("00055").ShouldBe("00055");
            OrganizationUnit.GetLastUnitCode("00019.00055.00001").ShouldBe("00001");
        }

        [Fact]
        public void TestGetParentCode()
        {
            OrganizationUnit.GetParentCode("00055").ShouldBe(null);
            OrganizationUnit.GetParentCode("00019.00055.00001").ShouldBe("00019.00055");
        }
    }
}