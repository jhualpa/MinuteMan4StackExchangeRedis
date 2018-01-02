namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivityMinuteTrackTests : UserActivityHourTrackTests
    {
        public UserActivityMinuteTrackTests()
            : this(AnalyticsTimeframe.Minute, nameof(UserActivityMinuteTrackTests))
        {
        }

        protected UserActivityMinuteTrackTests(
            AnalyticsTimeframe timeframe, string prefix)
            : base(timeframe, prefix)
        {
        }

        [Fact]
        public async Task CreatesMinuteEntry()
        {
            await TestExists(AnalyticsTimeframe.Minute);
        }
    }
}