using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minuteman.Helpers;
using Minuteman.Settings;
using Minuteman.Subscription;

namespace Minuteman.Abstract
{
    public abstract class Activity<TInfo> : IActivity<TInfo>
        where TInfo : Info
    {
        #region PRIVATE FIELDS

        protected readonly IClient _client;

        #endregion

        #region CONSTRUCTOR

        protected Activity(AnalyticsSettings settings, IClient client)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            Settings = settings;

            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _client = client;
        }

        #endregion

        #region PROTECTED PROPERTIES

        protected abstract string Prefix { get; }

        #endregion

        #region IActivity MEMBERS

        public AnalyticsSettings Settings { get; private set; }

        public virtual async Task<IEnumerable<string>> GetActivityNames(
            bool onlyPublished)
        {
            var activityKey = GenerateKey();

            if (onlyPublished)
            {
                activityKey += Settings.KeySeparator + "published";
            }

            var members = await _client.GetAllMembersInSetAsync(activityKey).ConfigureAwait(false);
            
            var result = members.Select(RemoveKeyPrefix)
                .OrderBy(n => n)
                .ToList();

            return result;
        }

        public virtual async Task<IEnumerable<AnalyticsTimeframe>> GetTimeframes(
            string activityName)
        {
            Validation.ValidateEventName(activityName);

            var key = GenerateKey() +
                Settings.KeySeparator +
                activityName;


            var members = await _client.GetAllMembersInSetAsync(key).ConfigureAwait(false);

            var result = members.Select(m => 
                (AnalyticsTimeframe)Enum.Parse(typeof(AnalyticsTimeframe), m))
                .ToList();

            return result;
        }

        public virtual async Task<long> Reset()
        {
            var wildcard = GenerateKey() + "*";
            // get keys in server database that include "key*" in their name
            var result = await _client.ResetKeyAsync(wildcard).ConfigureAwait(false);

            return result;
        }

        public virtual ISubscription<TInfo> CreateSubscription()
        {
            var prefix = GenerateKey() + Settings.KeySeparator;

            var subscription = new Subscription<TInfo>(prefix, _client);

            return subscription;
        }

        #endregion

        #region PROTECTED METHODS

        protected internal string GenerateKey(params string[] parts)
        {
            var prefix = Settings.KeyPrefix + Settings.KeySeparator + Prefix;

            if (!parts.Any())
            {
                return prefix;
            }

            var key = prefix +
                Settings.KeySeparator +
                string.Join(
                    Settings.KeySeparator,
                    parts.Select(part => part.ToUpperInvariant()));

            return key;
        }

        #endregion

        #region PRIVATE METHODS

        private string RemoveKeyPrefix(string value)
        {
            var fullPrefix = GenerateKey(Prefix);
            var index = value.IndexOf(fullPrefix, StringComparison.Ordinal);

            var result = index < 0 ? value : value.Substring(index);

            return result;
        }

        #endregion
    }
}