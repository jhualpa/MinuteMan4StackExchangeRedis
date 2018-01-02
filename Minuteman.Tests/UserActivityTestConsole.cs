using System;
using System.Linq;
using System.Threading.Tasks;
using Minuteman.Abstract;
using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;
using Xunit;

namespace Minuteman.Tests
{
    public class UserActivityTestConsole : IDisposable
    {
        private readonly IUserActivity userActivity;
        private NinjectFixture diFixture;

        public UserActivityTestConsole()
        {
            diFixture = new NinjectFixture();
            userActivity = new UserActivity(new AnalyticsSettings(AnalyticsTimeframe.Day, nameof(UserActivityTestConsole), ":"), diFixture.Client);
            userActivity.Reset().Wait();
        }

        public void Dispose()
        {
            userActivity.Reset().Wait();
        }

        [Fact]
        public async Task TrackLoginUserActivity()
        {
            await userActivity.Track("login", DateTime.UtcNow, 1).ConfigureAwait(false);            
        }

        [Fact]
        public async Task ReturnsTrackedLoginCount()
        {
            await userActivity.Track("login", DateTime.UtcNow, 1,5,9).ConfigureAwait(false);   
            var count = await userActivity.Report("login", DateTime.UtcNow).Count().ConfigureAwait(false);
            Assert.NotEqual(0, count);
        }

        [Fact]
        public async Task ReportExclusiveUser()
        {
            await userActivity.Track("login", DateTime.UtcNow, 1).ConfigureAwait(false);   
            var result = await userActivity.Report("login", DateTime.UtcNow).Includes(1).ConfigureAwait(false);
            Assert.True(result.First());
        }

        [Fact]
        public async Task ReportLoggedinnUsers2()
        {
            await userActivity.Track("login-gmail", DateTime.UtcNow, 1, 2).ConfigureAwait(false);
            await userActivity.Track("login-facebook", DateTime.UtcNow, 1, 3).ConfigureAwait(false);
            var logGmail = userActivity.Report("login-gmail", DateTime.UtcNow);
            var logFb = userActivity.Report("login-facebook", DateTime.UtcNow);
            var both = await (logGmail & logFb).Count();
            var eitherOrBoth = await (logGmail | logFb).Count();
            var eitherNotBoth = await (logGmail ^ logFb).Count();

            Assert.Equal(1, both);
            Assert.Equal(3, eitherOrBoth);
            Assert.Equal(2, eitherNotBoth);
        }
        
    }
}
