using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityXorCountTests : IDisposable
    {
        private readonly UserActivity userActivity;
        private NinjectFixture diFixture;

        public UserActivityXorCountTests()
        {
            diFixture = new NinjectFixture();
            userActivity = new UserActivity(new AnalyticsSettings(AnalyticsTimeframe.Year, nameof(UserActivityXorCountTests), ":"), diFixture.Client);

            userActivity.Reset().Wait();
        }

        public void Dispose()
        {
            userActivity.Reset().Wait();
        }

        [Fact]
        public async Task ReturnsSumOfExclusives()
        {
            var timestamp = DateTime.UtcNow;

            await Task.WhenAll(
                userActivity.Track("bought-apple", timestamp, 1, 2, 3, 4),
                userActivity.Track("bought-banana", timestamp, 3, 4, 5, 6)).ConfigureAwait(false);

            var apple = userActivity.Report("bought-apple", timestamp);
            var banana = userActivity.Report("bought-banana", timestamp);
            var both = apple ^ banana;
            var count = await both.Count();

            Assert.Equal(4, count);
        }        
    }
}