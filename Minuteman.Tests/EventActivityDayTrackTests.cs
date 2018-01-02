namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class EventActivityDayTrackTests : EventActivityMonthTrackTests
    {
        public EventActivityDayTrackTests() :
            this(AnalyticsTimeframe.Day, nameof(EventActivityDayTrackTests))
        {
        }

        protected EventActivityDayTrackTests(
            AnalyticsTimeframe timeframe, string prefix)
            : base(timeframe, prefix)
        {
        }

        [Fact]
        public async Task CreatesDayField()
        {
            await TestExists(AnalyticsTimeframe.Day);
        }
    }
}