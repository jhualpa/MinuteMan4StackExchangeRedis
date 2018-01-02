using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;

namespace Minuteman.Infrastructure.Configuration.Providers
{
    public class WebConfigProvider : IWebConfigProvider
    {
        public T GetAppSetting<T>(string key)
        {
            try
            {
                var appSetting = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrWhiteSpace(appSetting)) throw new Exception(String.Format("The key {0} was found, but it was empty", key));

                var converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)(converter.ConvertFromInvariantString(appSetting));
            }
            catch (Exception ex)
            {
                // This is to allow to execute the application without values. Only for debug purpuses.
#if AVOID_WEBCONFIG
                    return default(T);
#else
                throw new KeyNotFoundException(key, ex);
#endif
            }
        }

        public T GetConnectionString<T>(string key)
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings[key].ConnectionString;
                if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception(String.Format("The key {0} was found, but it was empty", key));

                var converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)(converter.ConvertFromInvariantString(connectionString));
            }
            catch (Exception ex)
            {
                // This is to allow to execute the application without values. Only for debug purpuses.
#if AVOID_WEBCONFIG
                    return default(T);
#else
                throw new KeyNotFoundException(key, ex);
#endif
            }
        }

        public Dictionary<string, string> GetAllAppSettings()
        {
            return ConfigurationManager.AppSettings.Cast<string>().ToDictionary(key => key, key => ConfigurationManager.AppSettings[key]);
        }

        public Dictionary<string, string> GetAllConnectionStrings()
        {
            return ConfigurationManager.ConnectionStrings.Cast<string>().ToDictionary(key => key, key => ConfigurationManager.ConnectionStrings[key].ConnectionString);
        }
    }
}
