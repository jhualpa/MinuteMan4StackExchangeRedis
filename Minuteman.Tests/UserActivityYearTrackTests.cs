namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityYearTrackTests : UserActivityTrackTests
    {
        public UserActivityYearTrackTests() :
            this(AnalyticsTimeframe.Year, nameof(UserActivityYearTrackTests))
        {
        }

        protected UserActivityYearTrackTests(
            AnalyticsTimeframe timeframe, string prefix)
            : base(timeframe, prefix)
        {
        }

        [Fact]
        public async Task CreatesYearEntry()
        {
            await TestExists(AnalyticsTimeframe.Year);
        }
    }
}