namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class EventActivityMonthTrackTests : EventActivityYearTrackTests
    {
        public EventActivityMonthTrackTests() :
            this(AnalyticsTimeframe.Month, nameof(EventActivityMonthTrackTests))
        {
        }

        protected EventActivityMonthTrackTests(
            AnalyticsTimeframe timeframe, string prefix)
            : base(timeframe, prefix)
        {
        }

        [Fact]
        public async Task CreatesMonthField()
        {
            await TestExists(AnalyticsTimeframe.Month);
        }
    }
}