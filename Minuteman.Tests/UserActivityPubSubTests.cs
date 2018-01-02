using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityPubSubTests : IDisposable
    {
        private readonly UserActivity activity;
        private NinjectFixture diFixture;

        public UserActivityPubSubTests()
        {
            diFixture = new NinjectFixture();
            activity = new UserActivity(new AnalyticsSettings(AnalyticsTimeframe.Year, nameof(UserActivityPubSubTests), ":"), diFixture.Client);
            activity.Reset().Wait();
        }

        public void Dispose()
        {
            activity.Reset().Wait();
        }

        [Fact]
        public async Task ExchangesMessage()
        {
            const string EventName = "login";
            var timestamp = DateTime.UtcNow;

            var signal = new ManualResetEvent(false);

            var subscription = activity.CreateSubscription();

            await subscription.Subscribe(
                EventName,
                e =>
                    {
                        Assert.Equal(EventName, e.ActivityName);
                        Assert.Equal(timestamp, e.Timestamp);
                        Assert.Contains(1, e.Users);
                        Assert.Contains(2, e.Users);
                        Assert.Contains(3, e.Users);
                        signal.Set();
                    });

            await activity.Track(EventName, true, 1, 2, 3);

            signal.WaitOne(TimeSpan.FromSeconds(1));

            await subscription.Unsubscribe(EventName);
        }       
    }
}