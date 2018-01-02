namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityMonthTrackTests : UserActivityYearTrackTests
    {
        public UserActivityMonthTrackTests() :
            this(AnalyticsTimeframe.Month, nameof(UserActivityMonthTrackTests))
        {
        }

        protected UserActivityMonthTrackTests(
            AnalyticsTimeframe timeframe, string prefix)
            : base(timeframe, prefix)
        {
        }

        [Fact]
        public async Task CreatesMonthEntry()
        {
            await TestExists(AnalyticsTimeframe.Month);
        }
    }
}