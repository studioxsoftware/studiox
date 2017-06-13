using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Mvc;
using StudioX.Timing;
using StudioX.Web.Mvc.ModelBinding.Binders;
using Shouldly;
using Xunit;

namespace StudioX.Web.Mvc.Tests.ModelBinders.Binders
{
    public class StudioXMvcDateTimeBinderTests
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
        public void DateTimeBinderUtcTests(string sourceDate)
        {
            Clock.Provider = ClockProviders.Utc;

            var resultDate = new DateTime(2016, 04, 13, 08, 58, 10, 526, Clock.Kind);
            var fields = new NameValueCollection { { "date", sourceDate } };
            var metaData = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(DateTime?));

            var binderContext = new ModelBindingContext
            {
                ModelName = "date",
                ModelMetadata = metaData,
                ValueProvider = new NameValueCollectionValueProvider(fields, null)
            };

            var boundDate = new StudioXMvcDateTimeBinder().BindModel(new ControllerContext(), binderContext) as DateTime?;
            boundDate.ShouldNotBe(null);
            boundDate.ShouldBe(resultDate);
            boundDate.Value.Kind.ShouldBe(Clock.Kind);
        }

        [Fact]
        public void DateTimeBinderLocalTests()
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
                DateTimeBinderLocalTestInternal(date);
            }
        }

        private void DateTimeBinderLocalTestInternal(string sourceDate)
        {
            Clock.Provider = ClockProviders.Local;
            var resultDate = new DateTime(2016, 04, 13, 08, 58, 10, 526, DateTimeKind.Utc).ToLocalTime();

            var fields = new NameValueCollection { { "date", sourceDate } };
            var metaData = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(DateTime?));

            var binderContext = new ModelBindingContext
            {
                ModelName = "date",
                ModelMetadata = metaData,
                ValueProvider = new NameValueCollectionValueProvider(fields, null)
            };

            var boundDate = new StudioXMvcDateTimeBinder().BindModel(new ControllerContext(), binderContext) as DateTime?;
            boundDate.ShouldNotBe(null);
            boundDate.ShouldBe(resultDate);
            boundDate.Value.Kind.ShouldBe(Clock.Kind);
        }
    }
}
