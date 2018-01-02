using System;
using System.Threading.Tasks;
using Minuteman.Abstract;
using Minuteman.Helpers;

namespace Minuteman.Subscription
{
    public class Subscription<TInfo> : ISubscription<TInfo>
        where TInfo : Info
    {
        #region PRIVATE FIELDS

        private readonly IClient _client;
        private readonly string _prefix;

        #endregion

        #region CONSTRUCTOR

        public Subscription(string prefix, IClient client)
        {
            Validation.ValidateString(
                prefix,
                ErrorMessages.Subscription_Constructor_Prefix_Required,
                "prefix");

            this._prefix = prefix;

            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            this._client = client;
        }

        #endregion

        #region ISubscription MEMBERS

        public virtual async Task Subscribe(string eventName, Action<TInfo> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var channel = Channel(eventName);

            await _client.SubscribeAsync<TInfo>(channel, action).ConfigureAwait(false);
        }

        public virtual async Task Unsubscribe(string eventName)
        {
            var channel = Channel(eventName);

            await _client.UnsubscribeAsync(channel).ConfigureAwait(false);
        }

        private string Channel(string eventName)
        {
            Validation.ValidateEventName(eventName);

            return _prefix + eventName.ToUpperInvariant();
        }

        #endregion

    }
}