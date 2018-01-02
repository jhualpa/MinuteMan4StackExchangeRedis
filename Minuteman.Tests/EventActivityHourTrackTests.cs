namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class EventActivityHourTrackTests : EventActivityDayTrackTests
    {
        public EventActivityHourTrackTests() :
            this(AnalyticsTimeframe.Hour, nameof(EventActivityHourTrackTests))
        {
        }

        protected EventActivityHourTrackTests(
            AnalyticsTimeframe timeframe, string prefix)
            : base(timeframe, prefix)
        {
        }

        [Fact]
        public async Task CreatesHourField()
        {
            await TestExists(AnalyticsTimeframe.Hour);
        }
    }
}