using System.Collections.Generic;

namespace Minuteman.Infrastructure.Configuration.Providers
{
    public interface IWebConfigProvider
    {
        T GetAppSetting<T>(string key);
        Dictionary<string, string> GetAllAppSettings();

        T GetConnectionString<T>(string key);
        Dictionary<string, string> GetAllConnectionStrings();
    }
}
