using System;

namespace Minuteman.Abstract
{
    public abstract class Info
    {
        public string ActivityName { get; set; }

        public AnalyticsTimeframe Timeframe { get; set; }

        public DateTime Timestamp { get; set; }        
    }
}