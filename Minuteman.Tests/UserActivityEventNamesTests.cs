using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityEventNamesTests : IDisposable
    {
        private readonly UserActivity userActivity;
        private NinjectFixture diFixture;

        public UserActivityEventNamesTests()
        {
            diFixture = new NinjectFixture();
            userActivity = new UserActivity(new AnalyticsSettings(AnalyticsTimeframe.Hour, nameof(UserActivityEventNamesTests), ":"), diFixture.Client);
            userActivity.Reset().Wait();
        }

        public void Dispose()
        {
            userActivity.Reset().Wait();
        }

        [Fact]
        public async Task ReturnsAllTrackedEventNames()
        {
            var timestamp = DateTime.UtcNow;
            var inputedEvents = new List<string>();

            for (var i = 1; i <= 5; i++)
            {
                await userActivity.Track("event-" + i, timestamp, i);
            }

            var outputedEvents = await userActivity.EventNames();

            inputedEvents.ForEach(n => Assert.Contains(n, outputedEvents));
        }        
    }
}