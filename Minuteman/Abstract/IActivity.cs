using System.Collections.Generic;
using System.Threading.Tasks;
using Minuteman.Settings;

namespace Minuteman.Abstract
{
    public interface IActivity<out TInfo> where TInfo : Info
    {
        AnalyticsSettings Settings { get; }

        Task<IEnumerable<string>> GetActivityNames(bool onlyPublished);

        Task<IEnumerable<AnalyticsTimeframe>> GetTimeframes(string activityName);

        Task<long> Reset();

        ISubscription<TInfo> CreateSubscription();
    }
}