using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minuteman.Abstract;
using StackExchange.Redis;
using Minuteman.Infrastructure.Serialization.Abstract;

namespace Minuteman.Redis
{
    /// <summary>
    /// This class is the wrapper of the redis client
    /// that provides a higher level interface to operate with string bit operations and hashes asynchronously
    /// that are used on Minuteman.
    /// It always connects to DB number 2
    /// </summary>
    public class Client : IClient
    {
        #region CONSTANTS

        private const int DB = 2;

        #endregion

        #region PRIVATE FIELDS

        private readonly ISerializer _serializer;
        private readonly IDatabase _database;
        private readonly ISubscriber _subscriber;
        private readonly IEnumerable<IServer> _servers;

        #endregion

        #region CONSTRUCTOR

        public Client(IDatabase database, ISubscriber subscriber, ISerializer serializer, IEnumerable<IServer> servers)
        {
            _database = database ?? throw new ArgumentNullException("database");
            _subscriber = subscriber ?? throw new ArgumentNullException("subscriber");
            _serializer = serializer ?? throw new ArgumentNullException("serializer");
            _servers = servers ?? throw new ArgumentNullException("servers");            
        }

        #endregion

        #region IClient Members

        /// <summary>
        /// Retrieves a string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A string.</returns>
        public async Task<string> GetStringAsync(string key)
        {
            var result = await _database.StringGetAsync(key).ConfigureAwait(false);
            return result.ToString();
        }

        /// <summary>
        /// Removes a redis key and its associated data structure.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A boolean that indicates the success of the operation.</returns>
        public async Task<bool> RemoveKeyAsync(string key)
        {
            return await _database.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds an element to the set identified by the key.
        /// </summary>
        /// <param name="key">The key that identifies the set.</param>
        /// <param name="value">The element to be added.</param>
        /// <returns>A boolean that indicates the success of the operation.</returns>
        public Task<bool> AddToSetAsync(string key, string value)
        {
            return _database.SetAddAsync(key, value);
        }

        /// <summary>
        /// Gets all members in a set.
        /// </summary>
        /// <param name="key">The key that identifies the set.</param>
        /// <returns>An enumerator over a collection of the elements that belong to the set.</returns>
        public async Task<IEnumerable<string>> GetAllMembersInSetAsync(string key)
        {
            var result = await _database.SetMembersAsync(key).ConfigureAwait(false);
            var members = result.Select(r => r.ToString());

            return members;
        }

        /// <summary>
        /// Resets all redis keys that match a given pattern.
        /// </summary>
        /// <param name="pattern">The key pattern to match against</param>
        /// <returns>A number that represents the number of keys removed.</returns>
        public async Task<long> ResetKeyAsync(string pattern)
        {
            long result = 0;
            foreach (var s in _servers)
            {
                
                var keys = s.Keys(database: _database.Database , pattern: pattern).ToArray();

                result += await _database.KeyDeleteAsync(keys).ConfigureAwait(false);
            }

            return result;
        }

        public async Task<long> IncrementHashFieldAsync(string key, string field)
        {
            return await _database.HashIncrementAsync(key, field).ConfigureAwait(false);
        }

        public async Task<bool> ExistsHashFieldAsync(string key, string field)
        {
            return await _database.HashExistsAsync(key, field).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> GetHashFieldsAsync(string key, IEnumerable<string> fields)
        {
            var result = await _database.HashGetAsync(key, fields.Select(f => (RedisValue)f).ToArray()).ConfigureAwait(false);
            var values = result.Select(r => r.ToString());

            return values;
        }

        /// <summary>
        /// Publishes data to the specified channel.
        /// </summary>
        /// <typeparam name="T">The type to publish.</typeparam>
        /// <param name="channel">The channel name.</param>
        /// <param name="data">The data to be published.</param>
        /// <returns>The number of clients that received the message.</returns>
        public async Task<long> PublishAsync<T>(string channel, T data)
        {
            return await _subscriber.PublishAsync(channel, _serializer.Serialize<T>(data)).ConfigureAwait(false);
        }

        public async Task<bool> SetStringBitAsync(string key, long offset, bool bit)
        {
            return await _database.StringSetBitAsync(key, offset, bit).ConfigureAwait(false);
        }

        public async Task<bool> GetStringBitAsync(string key, long offset)
        {
            return await _database.StringGetBitAsync(key, offset).ConfigureAwait(false);
        }

        public async Task<long> CountStringBitsAsync(string key)
        {
            return await _database.StringBitCountAsync(key).ConfigureAwait(false);
        }

        public async Task<long> BitwiseAndAsync(string key, IEnumerable<string> keys)
        {
            return await _database.StringBitOperationAsync(Bitwise.And, key, keys.Select(k => (RedisKey)k).ToArray()).ConfigureAwait(false);
        }

        public async Task<long> BitwiseOrAsync(string key, IEnumerable<string> keys)
        {
            return await _database.StringBitOperationAsync(Bitwise.Or, key, keys.Select(k => (RedisKey)k).ToArray()).ConfigureAwait(false);
        }

        public async Task<long> BitwiseXorAsync(string key, IEnumerable<string> keys)
        {
            return await _database.StringBitOperationAsync(Bitwise.Xor, key, keys.Select(k => (RedisKey)k).ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Subscribes the specified channel.
        /// </summary>
        /// <typeparam name="T">The type to publish.</typeparam>
        /// <param name="channel">The channel name.</param>
        /// <param name="onMessage">A handler to act upon mesage reception.</param>
        public async Task SubscribeAsync<T>(string channel, Action<T> onMessage)
        {
            await _subscriber.SubscribeAsync(channel, (theChannel, theMessage) => onMessage(_serializer.Deserialize<T>(theMessage))).ConfigureAwait(false);
        }

        /// <summary>
        /// Unsubscribes the redis client from the specified channel.
        /// </summary>
        /// <param name="channel">The channel name.</param>
        public async Task UnsubscribeAsync(string channel)
        {
            await _subscriber.UnsubscribeAsync(channel).ConfigureAwait(false);
        }

        #endregion
    }
}
