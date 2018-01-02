using System;
using System.Threading.Tasks;
using Minuteman.Subscription;

namespace Minuteman.Abstract
{
    public interface IEventActivity : IActivity<EventActivitySubscriptionInfo>
    {
        Task<long> Track(
            string eventName,
            AnalyticsTimeframe timeframe,
            DateTime timestamp,
            bool publishable);

        Task<long[]> Count(
            string eventName,
            DateTime startTimestamp,
            DateTime endTimestamp,
            AnalyticsTimeframe timeframe);
    }
}