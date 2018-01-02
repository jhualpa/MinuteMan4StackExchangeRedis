using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityIncludesTest : IDisposable
    {
        private const string EventName = "my-user-activity-include-event";

        private readonly UserActivity userActivity;
        private NinjectFixture diFixture;

        public UserActivityIncludesTest()
        {
            diFixture = new NinjectFixture();
            userActivity = new UserActivity(new AnalyticsSettings(AnalyticsTimeframe.Year,nameof(UserActivityIncludesTest), ":"), diFixture.Client);
            userActivity.Reset().Wait();
        }

        public void Dispose()
        {
            userActivity.Reset().Wait();
        }

        [Fact]
        public async Task RetunsTrueIfPreviouslyStored()
        {
            var timestamp = DateTime.UtcNow;

            await userActivity.Track(EventName, timestamp, 97);
            await userActivity.Track(EventName, timestamp, 98);
            await userActivity.Track(EventName, timestamp, 99);

            var result = await userActivity.Report(EventName, timestamp)
                .Includes(98);

            Assert.True(result.First());
        }

        [Fact]
        public async Task ReturnFalseIfPreviouslyNotStored()
        {
            var timestamp = DateTime.UtcNow;

            await userActivity.Track(EventName, timestamp, 101);
            await userActivity.Track(EventName, timestamp, 103);

            var result = await userActivity.Report(EventName, timestamp)
                .Includes(102);

            Assert.False(result.First());
        }        
    }
}