using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Minuteman.Abstract;
using Minuteman.Helpers;

namespace Minuteman.Reports
{
    public class UserActivityReport
    {
        #region CONSTRUCTOR

        public UserActivityReport(IClient client, string key)
        {
            Validation.ValidateString(
                key,
                ErrorMessages.UserActivityReport_Constructor_Required,
                "key");

            Client = client;
            Key = key;
        }

        #endregion

        #region PROTECTED PROPERTIES

        protected internal IClient Client { get; private set; }

        protected internal string Key { get; private set; }

        #endregion

        #region STATIC METHODS

        protected static UserActivityReport BitwiseAnd(
            UserActivityReport left,
            UserActivityReport right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            var key = left.Key + "&" + right.Key;

            var bitwiseAnd = new Func<IEnumerable<string>, Task<long>>(async (keys) =>
            {
                var client = left.Client;
                return await client.BitwiseAndAsync(key, keys).ConfigureAwait(false);
            });

            return new CompositeUserActivityReport(left.Client, key, bitwiseAnd, left, right);
        }

        protected static UserActivityReport BitwiseOr(
            UserActivityReport left,
            UserActivityReport right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            var key = left.Key + "|" + right.Key;

            var bitwiseOr = new Func<IEnumerable<string>, Task<long>>(async (keys) =>
            {
                var client = left.Client;
                return await client.BitwiseOrAsync(key, keys).ConfigureAwait(false);
            });

            return new CompositeUserActivityReport(left.Client, key, bitwiseOr, left, right);
        }

        protected static UserActivityReport BitwiseXor(
            UserActivityReport left,
            UserActivityReport right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            var key = left.Key + "^" + right.Key;

            var bitwiseXor = new Func<IEnumerable<string>, Task<long>>(async (keys) =>
            {
                var client = left.Client;
                return await client.BitwiseXorAsync(key, keys).ConfigureAwait(false);
            });

            return new CompositeUserActivityReport(left.Client, key, bitwiseXor, left, right);
        }

        #endregion

        #region OPERATOR OVERLOADS

        public static UserActivityReport operator &(
            UserActivityReport left,
            UserActivityReport right)
        {
            return BitwiseAnd(left, right);
        }

        public static UserActivityReport operator |(
            UserActivityReport left,
            UserActivityReport right)
        {
            return BitwiseOr(left, right);
        }

        public static UserActivityReport operator ^(
            UserActivityReport left,
            UserActivityReport right)
        {
            return BitwiseXor(left, right);
        }

        #endregion

        #region PUBLIC MEMBERS

        public virtual async Task<bool[]> Includes(params long[] users)
        {
            Validation.ValidateUsers(users);

            return await InternalIncludes(users).ConfigureAwait(false);
            
        }

        public virtual async Task<long> Count()
        {
            return await InternalCount().ConfigureAwait(false);
        }

        public virtual async Task<bool> Remove()
        {
            bool result = await Client.RemoveKeyAsync(Key).ConfigureAwait(false);

            return result;
        }

        #endregion

        #region INTERNAL MEMBERS

        internal async Task<bool[]> InternalIncludes(params long[] users)
        {
            var result = await Task.WhenAll(
                users.Select(user =>
                    Client.GetStringBitAsync(Key, user))).ConfigureAwait(false);

            return result;
        }

        internal async Task<long> InternalCount()
        {
            var result = await Client.CountStringBitsAsync(Key).ConfigureAwait(false);

            return result;
        }

        #endregion

    }
}