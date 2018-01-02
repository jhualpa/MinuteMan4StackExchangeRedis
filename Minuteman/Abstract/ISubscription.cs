using System;
using System.Threading.Tasks;

namespace Minuteman.Abstract
{
    public interface ISubscription<out TInfo> where TInfo : Info
    {
        Task Subscribe(string eventName, Action<TInfo> action);

        Task Unsubscribe(string eventName);
    }
}