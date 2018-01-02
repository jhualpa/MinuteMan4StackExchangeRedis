using System;
using System.Linq;
using System.Threading.Tasks;
using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;
using Xunit;

namespace Minuteman.Tests
{
    public class EventActivityTestConsole : IDisposable
    {
        private readonly EventActivity eventActivity;
        private NinjectFixture diFixture;

        public EventActivityTestConsole()
        {
            diFixture = new NinjectFixture();
            eventActivity = new EventActivity(new AnalyticsSettings(AnalyticsTimeframe.Day, nameof(EventActivityTestConsole), ":"), diFixture.Client);
            eventActivity.Reset().Wait();
        }

        public void Dispose()
        {
            eventActivity.Reset().Wait();
        }

        [Fact]
        public async Task TrackApiUsage()
        {
            var increment = await eventActivity.Track("clientApiKey").ConfigureAwait(false);
            
            Assert.NotEqual(0, increment);
        }

        [Fact]
        public async Task CountApiUsage()
        {
            await eventActivity.Track("clientApiKey").ConfigureAwait(false);
            await eventActivity.Track("clientApiKey").ConfigureAwait(false);

            var counts = await eventActivity.Count("clientApiKey", startTimestamp: DateTime.Now, endTimestamp: DateTime.Now).ConfigureAwait(false);


            Assert.Equal(2, counts.First());
        }  
    }
}
