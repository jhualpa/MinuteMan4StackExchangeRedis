namespace Minuteman.Tests
{
    using System.Threading.Tasks;

    using Xunit;

    public class UserActivitySecondTrackTests : UserActivityMinuteTrackTests
    {
        public UserActivitySecondTrackTests()
            : base(AnalyticsTimeframe.Second, nameof(UserActivitySecondTrackTests))
        {
        }

        [Fact]
        public async Task CreatesSecondEntry()
        {
            await TestExists(AnalyticsTimeframe.Second);
        }
    }
}