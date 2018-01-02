namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class EventActivityYearTrackTests : EventActivityTrackTests
    {
        public EventActivityYearTrackTests() : this(AnalyticsTimeframe.Year, nameof(EventActivityYearTrackTests))
        {
        }

        protected EventActivityYearTrackTests(
            AnalyticsTimeframe timeframe, string prefix)
            : base(timeframe, prefix)
        {
        }

        [Fact]
        public async Task CreatesYearField()
        {
            await TestExists(AnalyticsTimeframe.Year);
        }
    }
}