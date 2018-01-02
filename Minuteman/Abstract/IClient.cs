using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Minuteman.Abstract
{
    public interface IClient
    {
        Task<string> GetStringAsync(string key);
        Task<bool> AddToSetAsync(string key, string value);
        Task<IEnumerable<string>> GetAllMembersInSetAsync(string key);
        Task<long> ResetKeyAsync(string pattern);
        Task<bool> RemoveKeyAsync(string key);
        Task SubscribeAsync<T>(string channel, Action<T> onMessage);
        Task UnsubscribeAsync(string channel);
        Task<long> IncrementHashFieldAsync(string key, string field);
        Task<IEnumerable<string>> GetHashFieldsAsync(string key, IEnumerable<string> fields);
        Task<bool> ExistsHashFieldAsync(string key, string field);
        Task<long> PublishAsync<T>(string channel, T data);
        Task<bool> SetStringBitAsync(string key, long offset, bool bit);
        Task<bool> GetStringBitAsync(string key, long offset);
        Task<long> CountStringBitsAsync(string key);
        Task<long> BitwiseAndAsync(string key, IEnumerable<string> keys);
        Task<long> BitwiseOrAsync(string key, IEnumerable<string> keys);
        Task<long> BitwiseXorAsync(string key, IEnumerable<string> keys);
    }
}
