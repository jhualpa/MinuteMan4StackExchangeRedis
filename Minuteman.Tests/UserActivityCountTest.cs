using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityCountTest : IDisposable
    {
        private const string EventName = "my-user-activity-count-event";
        private readonly UserActivity userActivity;
        private NinjectFixture diFixture;

        public UserActivityCountTest()
        {
            diFixture = new NinjectFixture();
            userActivity = new UserActivity(new AnalyticsSettings(AnalyticsTimeframe.Year, nameof(UserActivityCountTest), ":"), diFixture.Client);
            userActivity.Reset().Wait();
        }

        [Fact]
        public async Task ReturnsTrackedEventsCount()
        {
            var timestamp = DateTime.UtcNow;

            await userActivity.Track(EventName, timestamp, 100, 204, 1002, 3).ConfigureAwait(false);

            var count = await userActivity.Report(EventName, timestamp).Count().ConfigureAwait(false);

            Assert.Equal(4, count);
        }

        public void Dispose()
        {
            userActivity.Reset().Wait();
        }        
    }
}