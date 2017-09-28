using System;
using StudioX.Extensions;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Extensions
{
    public class DayOfWeekExtensionsTests
    {
        [Fact]
        public void WeekendWeekdayTest()
        {
            DayOfWeek.Monday.IsWeekday().ShouldBe(true);
            DayOfWeek.Monday.IsWeekend().ShouldBe(false);

            DayOfWeek.Saturday.IsWeekend().ShouldBe(true);
            DayOfWeek.Saturday.IsWeekday().ShouldBe(false);

            var datetime1 = new DateTime(2014, 10, 5, 16, 37, 42); //Sunday
            var datetime2 = new DateTime(2014, 10, 7, 16, 37, 42); //Tuesday

            datetime1.DayOfWeek.IsWeekend().ShouldBe(true);
            datetime2.DayOfWeek.IsWeekend().ShouldBe(false);

            datetime1.DayOfWeek.IsWeekday().ShouldBe(false);
            datetime2.DayOfWeek.IsWeekday().ShouldBe(true);
        }
    }
}