﻿using Minuteman.Activities;
using Minuteman.Extensions;
using Minuteman.Settings;

namespace Minuteman.Tests
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityXorIncludesTests : IDisposable
    {
        private readonly UserActivity userActivity;
        private NinjectFixture diFixture;

        public UserActivityXorIncludesTests()
        {
            diFixture = new NinjectFixture();
            userActivity = new UserActivity(
                new AnalyticsSettings(AnalyticsTimeframe.Year, nameof(UserActivityXorIncludesTests), ":"), diFixture.Client);

            userActivity.Reset().Wait();
        }

        public void Dispose()
        {
            userActivity.Reset().Wait();
        }

        [Fact]
        public async Task IncludesOnlyExclusives()
        {
            var timestamp = DateTime.UtcNow;

            await Task.WhenAll(
                userActivity.Track("bought-apple", timestamp, 1, 2, 3, 4),
                userActivity.Track("bought-banana", timestamp, 3, 4, 5, 6)).ConfigureAwait(false);

            var apple = userActivity.Report("bought-apple", timestamp);
            var banana = userActivity.Report("bought-banana", timestamp);
            var both = apple ^ banana;

            var results = await both.Includes(0, 1, 2, 3, 4, 5, 6, 7);

            for (var i = 0; i < results.Length; i++)
            {
                if ((i == 0) || (i > 2 && i < 5) || (i == 7))
                {
                    Assert.False(results[i]);
                }
                else
                {
                    Assert.True(results[i]);
                }
            }
        }        
    }
}