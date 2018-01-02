using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minuteman.Abstract;
using Minuteman.Extensions;
using Minuteman.Helpers;
using Minuteman.Settings;
using Minuteman.Subscription;

namespace Minuteman.Activities
{
    public class EventActivity : Activity<EventActivitySubscriptionInfo>, IEventActivity
    {
        #region STATIC FIELDS

        private static readonly string EventsKeyName = typeof(EventActivity).Name;

        #endregion

        #region CONSTRUCTOR

        public EventActivity(AnalyticsSettings settings, IClient client)
            : base(settings, client)
        {
        }

        #endregion

        #region PROTECTED PROPERTIES

        protected override string Prefix
        {
            get { return EventsKeyName; }
        }

        #endregion

        #region IEventActivity MEMBERS

        public virtual async Task<long> Track(
            string eventName,
            AnalyticsTimeframe timeframe,
            DateTime timestamp,
            bool publishable)
        {
            // There are three tasks that we have to do here.
            // First, we have to maintain a list of events that has 
            // been tracked, so that, the same event list can be 
            // returned in the EventName method. Next. we also have to
            // maintain another list that is for time frames of the event, 
            // please note that we are only going to maintain
            // the explicit timeframes, the Timeframes method returns the 
            // explicit tracked timeframes for a given event name.
            // Second, increase the count for the matching timeframe. And at
            // last publish the event to redis so that the subscriber can be 
            // notified.
            Validation.ValidateEventName(eventName);

            var eventsKey = GenerateKey();
            var publishedEventsKey = eventsKey +
                Settings.KeySeparator +
                "published";

            var key = GenerateKey(eventName, timeframe.ToString());
            var fields = GenerateTimeframeFields(timeframe, timestamp).ToList();

            var eventTasks = new List<Task>
            {
                _client.AddToSetAsync(eventsKey, eventName),
                _client.AddToSetAsync(eventsKey + Settings.KeySeparator + eventName,
                    timeframe.ToString())
            };

            if (publishable)
            {
                eventTasks.Add(
                    _client.AddToSetAsync(publishedEventsKey, eventName));
            }

            await Task.WhenAll(eventTasks).ConfigureAwait(false);

            var fieldTasks = fields
                .Select(field => _client.IncrementHashFieldAsync(key, field))
                .ToList();

            var counts = await Task.WhenAll(fieldTasks).ConfigureAwait(false);
            var count = counts.ElementAt((int)timeframe);

            if (!publishable)
            {
                return count;
            }

            var channel = eventsKey +
                Settings.KeySeparator +
                eventName.ToUpperInvariant();

            var payload = new EventActivitySubscriptionInfo
            {
                ActivityName = eventName,
                Timestamp = timestamp,
                Timeframe = timeframe,
                Count = counts.ElementAt((int)timeframe)
            };

            await _client.PublishAsync(channel, payload).ConfigureAwait(false);

            return count;
            
        }

        public virtual async Task<long[]> Count(
            string eventName,
            DateTime startTimestamp,
            DateTime endTimestamp,
            AnalyticsTimeframe timeframe)
        {
            Validation.ValidateEventName(eventName);

            var dates = startTimestamp.Range(endTimestamp, timeframe);
            var key = GenerateKey(eventName, timeframe.ToString());

            var fields = dates.Select(d =>
                GenerateTimeframeFields(timeframe, d)
                    .ElementAt((int) timeframe))
                .Select(e => e).ToArray();

            IEnumerable<string> values = await _client.GetHashFieldsAsync(key, fields).ConfigureAwait(false);
            
            var result = values.Select(value =>
                    String.IsNullOrEmpty(value) ? 0L : long.Parse(value))
                .ToArray();

            return result;
        }

        #endregion

        #region INTERNAL METHODS

        internal IEnumerable<string> GenerateTimeframeFields(
            AnalyticsTimeframe timeframe,
            DateTime timestamp)
        {
            yield return timestamp.FormatYear();

            var separator = Settings.KeySeparator;
            var type = (int)timeframe;

            if (type > (int)AnalyticsTimeframe.Year)
            {
                yield return timestamp.FormatYear() +
                    separator +
                    timestamp.FormatMonth();
            }

            if (type > (int)AnalyticsTimeframe.Month)
            {
                yield return timestamp.FormatYear() +
                    separator +
                    timestamp.FormatMonth() +
                    separator +
                    timestamp.FormatDay();
            }

            if (type > (int)AnalyticsTimeframe.Day)
            {
                yield return timestamp.FormatYear() +
                    separator +
                    timestamp.FormatMonth() +
                    separator +
                    timestamp.FormatDay() +
                    separator +
                    timestamp.FormatHour();
            }

            if (type > (int)AnalyticsTimeframe.Hour)
            {
                yield return timestamp.FormatYear() +
                    separator +
                    timestamp.FormatMonth() +
                    separator +
                    timestamp.FormatDay() +
                    separator +
                    timestamp.FormatHour() +
                    separator +
                    timestamp.FormatMinute();
            }

            if (type > (int)AnalyticsTimeframe.Minute)
            {
                yield return timestamp.FormatYear() +
                    separator +
                    timestamp.FormatMonth() +
                    separator +
                    timestamp.FormatDay() +
                    separator +
                    timestamp.FormatHour() +
                    separator +
                    timestamp.FormatMinute() +
                    separator +
                    timestamp.FormatSecond();
            }
        }

        #endregion

    }
}