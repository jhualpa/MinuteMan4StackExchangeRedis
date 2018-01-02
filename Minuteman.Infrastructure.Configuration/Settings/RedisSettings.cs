using Minuteman.Infrastructure.Configuration.Providers;
using System.Collections.Generic;

namespace Minuteman.Infrastructure.Configuration.Settings
{
    public class RedisSettings : IRedisSettings
    {
        #region PRIVATE INSTANCE FIELDS 

        private readonly IWebConfigProvider _webConfigProvider;       

        private IDictionary<RedisKey, string> _keys = new Dictionary<RedisKey, string>
        {
            { RedisKey.Login, "login" },
            { RedisKey.ClientApiKey, "clientApiKey" }
        };

        private IDictionary<RedisChannel, string> _channels = new Dictionary<RedisChannel, string>();

        #endregion

        #region CONSTRUCTORS 

        public RedisSettings(IWebConfigProvider webConfigProvider)
        {
            _webConfigProvider = webConfigProvider;           
        }

        #endregion

        #region PUBLIC PROPERTIES 

        public string Configuration
        {
            get { return _webConfigProvider.GetConnectionString<string>("RedisServerSettings"); }
        }

        #endregion

        #region PUBLIC METHODS 

        public string Key(RedisKey key)
        {
            return _keys[key];
        }

        public string Channel(RedisChannel channel)
        {
            return _channels[channel];
        }

        #endregion
    }
}
