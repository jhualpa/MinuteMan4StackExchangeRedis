using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityTrackTests : IDisposable
    {
        protected const string EventName = "my-user-activity-track-event";
        protected static readonly DateTime Timestamp = DateTime.UtcNow;
        private readonly NinjectFixture diFixture;

        public UserActivityTrackTests() :
            this(nameof(UserActivityTrackTests))
        {
        }

        protected UserActivityTrackTests(string prefix)
        {
            diFixture = new NinjectFixture();
            UserActivity = new UserActivity(new AnalyticsSettings(AnalyticsTimeframe.Hour, prefix, ":"), diFixture.Client);
            UserActivity.Reset().Wait();
            UserActivity.Track(EventName, Timestamp, 1, 2, 3).Wait();
        }

        protected UserActivityTrackTests(AnalyticsTimeframe timeframe, string prefix)
        {
            diFixture = new NinjectFixture();
            UserActivity = new UserActivity(new AnalyticsSettings(timeframe, prefix, ":"), diFixture.Client);
            UserActivity.Reset().Wait();
            UserActivity.Track(EventName, Timestamp, 1, 2, 3).Wait();
        }

        protected UserActivity UserActivity { get; private set; }

        public void Dispose()
        {
            UserActivity.Reset().Wait();
        }

        protected async Task TestExists(AnalyticsTimeframe timeframe)
        {
            var key = UserActivity.GenerateEventTimeframeKeys(
                EventName,
                timeframe,
                Timestamp)
                .ElementAt((int)timeframe);

            var result = await diFixture.Client.GetStringAsync(key);

            Assert.NotNull(result);
        }        

        [Fact]
        public async Task AddsEventNameInTheEventsMember()
        {
            var events = await UserActivity.EventNames().ConfigureAwait(false);

            Assert.Contains(EventName, events);
        }
    }
}
