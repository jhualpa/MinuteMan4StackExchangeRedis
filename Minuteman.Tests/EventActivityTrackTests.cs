using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Xunit;

    public abstract class EventActivityTrackTests : IDisposable
    {
        protected const string EventName = "my-activity-track-event";
        protected static readonly DateTime Timestamp = DateTime.UtcNow;
        private NinjectFixture diFixture;

        protected EventActivityTrackTests(AnalyticsTimeframe timeframe, string prefix)
        {
            diFixture = new NinjectFixture();
            EventActivity = new EventActivity(new AnalyticsSettings(timeframe, prefix, ":"), diFixture.Client);
            EventActivity.Reset().Wait();
            EventActivity.Track(EventName, Timestamp).Wait();
        }

        protected EventActivity EventActivity { get; private set; }

        [Fact]
        public async Task AddsEventNameInTheEventsMember()
        {
            var events = await EventActivity.EventNames();

            Assert.Contains(EventName, events);
        }

        public void Dispose()
        {
            EventActivity.Reset().Wait();
        }

        protected async Task TestExists(AnalyticsTimeframe timeframe)
        {
            var key = EventActivity.GenerateKey(
                EventName,
                EventActivity.Settings.Timeframe.ToString());

            var field = EventActivity.GenerateTimeframeFields(
                timeframe,
                Timestamp)
                .ElementAt((int)timeframe);

            var result = await diFixture.Client.ExistsHashFieldAsync(key, field);

            Assert.True(result);
            
        }       
    }
}