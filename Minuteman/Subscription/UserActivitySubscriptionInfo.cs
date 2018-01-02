using System.Collections.Generic;
using Minuteman.Abstract;

namespace Minuteman.Subscription
{
    public class UserActivitySubscriptionInfo : Info
    {
       public IEnumerable<long> Users { get; set; }
    }
}