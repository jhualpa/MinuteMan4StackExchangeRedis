using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    public class EventActivityResetTests : IDisposable
    {
        private readonly EventActivity eventActivity;
        private NinjectFixture diFixture;

        public EventActivityResetTests()
        {
            diFixture = new NinjectFixture();
            eventActivity = new EventActivity(new AnalyticsSettings(AnalyticsTimeframe.Year, nameof(EventActivityResetTests), ":"), diFixture.Client);
            eventActivity.Reset().Wait();
        }

        [Fact]
        public async Task RemovesAllTrackedEvents()
        {
            await Task.WhenAll(
                eventActivity.Track("foo"),
                eventActivity.Track("bar"),
                eventActivity.Track("baz"));

            var count = await eventActivity.Reset();

            // 3 tracked events, 3 for the drilldowns and 1 for event names
            Assert.Equal(7, count);
        }

        public void Dispose()
        {
            eventActivity.Reset().Wait();
        }
    }
}