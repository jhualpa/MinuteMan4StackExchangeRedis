using System;
using System.Threading.Tasks;
using Minuteman.Reports;
using Minuteman.Subscription;

namespace Minuteman.Abstract
{
    public interface IUserActivity : IActivity<UserActivitySubscriptionInfo>
    {
        Task Track(
            string userActivityName,
            AnalyticsTimeframe timeframe,
            DateTime timestamp,
            bool publishable,
            params long[] users);

        UserActivityReport Report(
            string userActivityName,
            AnalyticsTimeframe timeframe,
            DateTime timestamp);
    }
}