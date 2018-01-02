using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class EventActivityPubSubTests : IDisposable
    {
        private readonly EventActivity activity;
        private NinjectFixture diFixture;

        public EventActivityPubSubTests()
        {
            diFixture = new NinjectFixture();
            activity = new EventActivity(new AnalyticsSettings(AnalyticsTimeframe.Year, nameof(EventActivityPubSubTests), ":"), diFixture.Client);
            activity.Reset().Wait();
        }

        [Fact]
        public async Task ExchangesMessage()
        {
            const string EventName = "order-placed";
            var timestamp = DateTime.UtcNow;

            var signal = new ManualResetEvent(false);

            var subscription = activity.CreateSubscription();

            await subscription.Subscribe(EventName, e =>
                {
                    Assert.Equal(EventName, e.ActivityName);
                    Assert.Equal(timestamp, e.Timestamp);
                    Assert.Equal(AnalyticsTimeframe.Year, e.Timeframe);
                    Assert.Equal(1, e.Count);
                    signal.Set();
                });

            await activity.Track(EventName, timestamp, true);

            signal.WaitOne(TimeSpan.FromSeconds(1));

            await subscription.Unsubscribe(EventName);
        }

        public void Dispose()
        {
            activity.Reset().Wait();
        }       
    }
}