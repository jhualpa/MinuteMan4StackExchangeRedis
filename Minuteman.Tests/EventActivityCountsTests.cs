using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Xunit;

    public class EventActivityCountsTests : IDisposable
    {
        private readonly EventActivity eventActivity;
        private NinjectFixture diFixture;

        public EventActivityCountsTests()
        {
            diFixture = new NinjectFixture();
            eventActivity = new EventActivity(new AnalyticsSettings(AnalyticsTimeframe.Hour, nameof(EventActivityCountsTests), ":"), diFixture.Client);
            eventActivity.Reset().Wait();
        }

        [Fact]
        public async void ReturnsCounts()
        {
            const string EventName = "my-activity-count-event";
            var now = DateTime.UtcNow;

            await Task.WhenAll(
                eventActivity.Track(EventName, now),
                eventActivity.Track(EventName, now)).ConfigureAwait(false);

            var counts = await eventActivity.Count(EventName, now, now);

            Assert.Equal(2, counts.First());
        }

        [Fact]
        public async void ReturnsZeroWhenNotTracked()
        {
            const string EventName = "my-activity-count-event-2";
            var now = DateTime.UtcNow;

            await Task.WhenAll(
                eventActivity.Track(EventName, now),
                eventActivity.Track(EventName, now.AddHours(2))).ConfigureAwait(false);

            var counts = await eventActivity.Count(
                EventName,
                now,
                now.AddHours(2));

            Assert.Equal(1, counts.First());
            Assert.Equal(0, counts.ElementAt(1));
            Assert.Equal(1, counts.Last());
        }

        public void Dispose()
        {
            eventActivity.Reset().Wait();
        }       
    }
}