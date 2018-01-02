namespace Minuteman.Settings
{
    public class AnalyticsSettings
    {
        #region PRIVATE FIELDS

        private const AnalyticsTimeframe DefaultTimeframe = AnalyticsTimeframe.Hour;
        private const string DefaultKeyPrefix = "minuteman";
        private const string DefaultKeySeparator = ":";

        #endregion


        #region CONSTRUCTORS

        public AnalyticsSettings() : 
            this(
            DefaultTimeframe,
            DefaultKeyPrefix,
            DefaultKeySeparator)
        {
        }
        
        public AnalyticsSettings(
            AnalyticsTimeframe timeframe)
            : this(timeframe, DefaultKeyPrefix, DefaultKeySeparator)
        {
        }

        public AnalyticsSettings(
            AnalyticsTimeframe timeframe,
            string keyPrefix,
            string keySeparator)
        {
            Timeframe = timeframe;
            KeyPrefix = keyPrefix ?? string.Empty;
            KeySeparator = keySeparator ?? string.Empty;
        }

        #endregion

        #region PUBLIC PROPERTIES

        public AnalyticsTimeframe Timeframe { get; private set; }

        public string KeyPrefix { get; private set; }

        public string KeySeparator { get; private set; }

        #endregion
    }
}