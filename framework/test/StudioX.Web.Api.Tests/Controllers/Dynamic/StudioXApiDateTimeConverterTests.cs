using System;
using System.Collections.Generic;
using StudioX.Json;
using StudioX.Timing;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace StudioX.Web.Api.Tests.Controllers.Dynamic
{
    public class StudioXApiDateTimeConverterTests
    {
        [Theory]
        [InlineData("2016-04-13T08:58:10.526Z")]
        [InlineData("2016-04-13T08:58:10.526")]
        [InlineData("2016-04-13 08:58:10.526Z")]
        [InlineData("2016-04-13 08:58:10.526")]
        [InlineData("2016-04-13T08:58:10.526+00:00")]
        [InlineData("2016-04-13T16:58:10.526+08:00")]
        [InlineData("2016-04-13T01:58:10.526-07:00")]
        [InlineData("2016-04-13 01:58:10.526AM-07:00")]
        [InlineData("2016-04-13 01:58:10.526PM+05:00")]
        public void DateTimeConverterUtcTests(string sourceDate)
        {
            Clock.Provider = ClockProviders.Utc;
            var resultDate = new DateTime(2016, 04, 13, 08, 58, 10, 526, DateTimeKind.Utc);

            var dto = JsonConvert.DeserializeObject<DateTimeConverterTestDto>("{date: \"" + sourceDate + "\"}", new StudioXDateTimeConverter());

            dto.Date.ShouldBe(resultDate);
            dto.Date.Kind.ShouldBe(DateTimeKind.Utc);
        }

        [Fact]
        public void DateTimeConverterLocalTests()
        {
            var testDates = new List<string>
            {
                "2016-04-13T08:58:10.526Z",
                "2016-04-13T08:58:10.526",
                "2016-04-13 08:58:10.526Z",
                "2016-04-13 08:58:10.526",
                "2016-04-13T08:58:10.526+00:00",
                "2016-04-13T16:58:10.526+08:00",
                "2016-04-13T01:58:10.526-07:00",
                "2016-04-13 01:58:10.526AM-07:00",
                "2016-04-13 01:58:10.526PM+05:00"
            };

            foreach (var testDate in testDates)
            {
                var date = DateTime.Parse(testDate).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
                DateTimeConverterLocalTestInternal(date);
            }
        }

        private void DateTimeConverterLocalTestInternal(string sourceDate)
        {
            Clock.Provider = ClockProviders.Local;
            var resultDate = new DateTime(2016, 04, 13, 08, 58, 10, 526, DateTimeKind.Utc).ToLocalTime();

            var dto = JsonConvert.DeserializeObject<DateTimeConverterTestDto>("{date: \"" + sourceDate + "\"}", new StudioXDateTimeConverter());

            dto.Date.ShouldBe(resultDate);
            dto.Date.Kind.ShouldBe(DateTimeKind.Local);
        }

        class DateTimeConverterTestDto
        {
            public DateTime Date { get; set; }
        }
    }
}
