using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minuteman.Abstract;
using Minuteman.Extensions;
using Minuteman.Helpers;
using Minuteman.Reports;
using Minuteman.Settings;
using Minuteman.Subscription;

namespace Minuteman.Activities
{
    public class UserActivity : Activity<UserActivitySubscriptionInfo>, IUserActivity
    {
        #region STATIC FIELDS

        private static readonly string ActivitiesKeyName = typeof(UserActivity).Name;

        #endregion


        #region CONSTRUCTORS

        public UserActivity(IClient client)
            : this(new AnalyticsSettings(), client)
        {
        }
        
        public UserActivity(AnalyticsSettings settings, IClient client)
            : base(settings, client)
        {
        }

        #endregion

        #region PROTECTED PROPERTIES

        protected override string Prefix
        {
            get { return ActivitiesKeyName; }
        }

        #endregion

        #region IUserActivity MEMBERS

        public virtual async Task Track(
            string userActivityName,
            AnalyticsTimeframe timeframe,
            DateTime timestamp,
            bool publishable,
            params long[] users)
        {
            Validation.ValidateEventName(userActivityName);
            Validation.ValidateUsers(users);

            var activitiesKey = GenerateKey();
            var publishedActivitiesKey = activitiesKey +
                Settings.KeySeparator +
                "published";

            string channel = null;
            UserActivitySubscriptionInfo payload = null;

            if (publishable)
            {
                channel = activitiesKey +
                    Settings.KeySeparator +
                    userActivityName.ToUpperInvariant();

                payload = new UserActivitySubscriptionInfo
                {
                    ActivityName = userActivityName,
                    Timestamp = timestamp,
                    Timeframe = timeframe,
                    Users = users
                };
            }

            var timeframeKeys = GenerateEventTimeframeKeys(
                userActivityName,
                timeframe,
                timestamp).ToList();

            var userActivityTasks = new List<Task>
            {
                _client.AddToSetAsync(activitiesKey, userActivityName),
                _client.AddToSetAsync(activitiesKey + Settings.KeySeparator + userActivityName, timeframe.ToString())
            };

            if (publishable)
            {
                userActivityTasks.Add(
                    _client.AddToSetAsync(publishedActivitiesKey, userActivityName));
            }

            await Task.WhenAll(userActivityTasks).ConfigureAwait(false);

            var bitTasks = new List<Task>();

            foreach (var timeframeKey in timeframeKeys)
            {
                bitTasks.AddRange(users.Select(user =>
                    _client.SetStringBitAsync(
                        timeframeKey,
                        user,
                        true)));
            }

            await Task.WhenAll(bitTasks).ConfigureAwait(false);

            if (publishable)
            {
                await _client.PublishAsync(channel, payload).ConfigureAwait(false);
            }
            
        }

        public virtual UserActivityReport Report(
            string userActivityName,
            AnalyticsTimeframe timeframe,
            DateTime timestamp)
        {
            Validation.ValidateEventName(userActivityName);

            var userActivityKey = GenerateEventTimeframeKeys(
                userActivityName, timeframe, timestamp).ElementAt((int)timeframe);

            return new UserActivityReport(this._client, userActivityKey);
        }

        #endregion

        #region INTERNAL MEMBERS

        internal IEnumerable<string> GenerateEventTimeframeKeys(
            string userActivityName,
            AnalyticsTimeframe timeframe,
            DateTime timestamp)
        {
            yield return GenerateKey(
                userActivityName,
                timestamp.FormatYear());

            var type = (int)timeframe;

            if (type > (int)AnalyticsTimeframe.Year)
            {
                yield return GenerateKey(
                    userActivityName, 
                    timestamp.FormatYear(),
                    timestamp.FormatMonth());
            }

            if (type > (int)AnalyticsTimeframe.Month)
            {
                yield return GenerateKey(
                    userActivityName, 
                    timestamp.FormatYear(),
                    timestamp.FormatMonth(),
                    timestamp.FormatDay());
            }

            if (type > (int)AnalyticsTimeframe.Day)
            {
                yield return GenerateKey(
                    userActivityName, 
                    timestamp.FormatYear(),
                    timestamp.FormatMonth(),
                    timestamp.FormatDay(),
                    timestamp.FormatHour());
            }

            if (type > (int)AnalyticsTimeframe.Hour)
            {
                yield return GenerateKey(
                    userActivityName,
                    timestamp.FormatYear(),
                    timestamp.FormatMonth(),
                    timestamp.FormatDay(),
                    timestamp.FormatHour(),
                    timestamp.FormatMinute());
            }

            if (type > (int)AnalyticsTimeframe.Minute)
            {
                yield return GenerateKey(
                    userActivityName,
                    timestamp.FormatYear(),
                    timestamp.FormatMonth(),
                    timestamp.FormatDay(),
                    timestamp.FormatHour(),
                    timestamp.FormatMinute(),
                    timestamp.FormatSecond());
            }
        }

        #endregion

    }
}