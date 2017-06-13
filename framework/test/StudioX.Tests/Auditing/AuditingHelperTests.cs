using System;
using StudioX.Auditing;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Auditing
{
    public class AuditingHelperTests
    {
        [Fact]
        public void IgnoredLongPropertiesLongShouldLongNotLongBeLongSerialized()
        {
            var json = new JsonNetAuditSerializer(new AuditingConfiguration {IgnoredTypes = { typeof(Exception) }})
                .Serialize(new AuditingHelperTestPersonDto
                {
                    FullName = "John Doe",
                    Age = 18,
                    School = new AuditingHelperTestSchoolDto
                    {
                        Name = "Crosswell Secondary",
                        Address = "Broadway Ave, West Bend"
                    },
                    Exception = new Exception("this should be ignored!")
                });

            json.ShouldBe("{\"fullName\":\"John Doe\",\"school\":{\"name\":\"Crosswell Secondary\"}}");
        }

        public class AuditingHelperTestPersonDto
        {
            public string FullName { get; set; }

            [DisableAuditing]
            public int Age { get; set; }

            public Exception Exception { get; set; }

            public AuditingHelperTestSchoolDto School { get; set; }
        }

        public class AuditingHelperTestSchoolDto
        {
            public string Name { get; set; }

            [DisableAuditing]
            public string Address { get; set; }
        }
    }
}
