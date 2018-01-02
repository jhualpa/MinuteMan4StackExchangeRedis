namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityHourTrackTests : UserActivityDayTrackTests
    {
        public UserActivityHourTrackTests()
            : this(AnalyticsTimeframe.Hour, nameof(UserActivityHourTrackTests))
        {
        }

        protected UserActivityHourTrackTests(
            AnalyticsTimeframe timeframe, string prefix)
            : base(timeframe, prefix)
        {
        }

        [Fact]
        public async Task CreatesHourEntry()
        {
            await TestExists(AnalyticsTimeframe.Hour);
        }
    }
}