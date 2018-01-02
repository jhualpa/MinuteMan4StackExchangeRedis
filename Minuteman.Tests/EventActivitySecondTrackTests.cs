namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class EventActivitySecondTrackTests : EventActivityMinuteTrackTests
    {
        public EventActivitySecondTrackTests() :
            base(AnalyticsTimeframe.Second, nameof(EventActivitySecondTrackTests))
        {
        }

        [Fact]
        public async Task CreatesSecondField()
        {
            await TestExists(AnalyticsTimeframe.Second);
        }
    }
}