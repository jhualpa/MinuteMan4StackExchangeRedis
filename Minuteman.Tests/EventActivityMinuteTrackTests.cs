namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class EventActivityMinuteTrackTests : EventActivityHourTrackTests
    {
        public EventActivityMinuteTrackTests() :
            this(AnalyticsTimeframe.Minute, nameof(EventActivityMinuteTrackTests))
        {
        }

        protected EventActivityMinuteTrackTests(
            AnalyticsTimeframe timeframe, string prefix)
            : base(timeframe, prefix)
        {
        }

        [Fact]
        public async Task CreatesMinuteField()
        {
            await TestExists(AnalyticsTimeframe.Minute);
        }
    }
}