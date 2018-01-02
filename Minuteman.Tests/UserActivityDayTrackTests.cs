namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityDayTrackTests : UserActivityMonthTrackTests
    {
        public UserActivityDayTrackTests()
            : this(AnalyticsTimeframe.Day, nameof(UserActivityDayTrackTests))
        {
        }

        protected UserActivityDayTrackTests(
            AnalyticsTimeframe timeframe, string prefix)
            : base(timeframe, prefix)
        {
        }

        [Fact]
        public async Task CreatesDayEntry()
        {
            await TestExists(AnalyticsTimeframe.Day);
        }
    }
}