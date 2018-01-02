using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityResetTests : IDisposable
    {
        private readonly UserActivity userActivity;
        private NinjectFixture diFixture;

        // For testing purposes
        public Task RunningTask { get; set; }

        public UserActivityResetTests()
        {
            diFixture = new NinjectFixture();
            userActivity = new UserActivity(new AnalyticsSettings(AnalyticsTimeframe.Year, nameof(UserActivityResetTests), ":"), diFixture.Client);
            userActivity.Reset().Wait();
        }

        [Fact]
        public async Task RemovesAllTrackedEvents()
        {
            await Task.WhenAll(
                userActivity.Track("foo", 1),
                userActivity.Track("bar", 2),
                userActivity.Track("baz", 3)).ConfigureAwait(false);


            var count = await userActivity.Reset().ConfigureAwait(false);

            // 3 tracked events, 3 for the drilldowns and 1 for event names
            Assert.Equal(7, count);
        }

        public void Dispose()
        {
            userActivity.Reset().Wait();
        }
    }
}