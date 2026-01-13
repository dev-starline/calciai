using CalciAI.Events;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CalciAI.Caching
{
    public static class RedisDatabaseExtensions
    {
        public async static Task<long> SetSetValuesAsync(this IDatabase database, string key, string[] values)
        {
            return await database.SetAddAsync(key, Array.ConvertAll(values, x => (RedisValue)x), CommandFlags.FireAndForget);
        }

        public async static Task<ISet<string>> GetSetValuesAsync(this IDatabase database, string key)
        {
            var data = await database.SetMembersAsync(key);
            return Array.ConvertAll(data, x => (string)x).ToHashSet();
        }

        public async static Task<string[]> GetStringValuesAsync(this IDatabase database, string[] keys)
        {
            var data = await database.StringGetAsync(Array.ConvertAll(keys, x => (RedisKey)x));
            return Array.ConvertAll(data, x => (string)x);
        }

        public async static Task<ISet<string>> ScanKeys(this IDatabase database, string pattern)
        {
            var keys = new HashSet<string>();

            long nextCursor = 0;

            do
            {
                var redisResult = await database.ExecuteAsync("SCAN", nextCursor.ToString(), "MATCH", pattern, "COUNT", "1000").ConfigureAwait(false);
                var innerResult = (RedisResult[])redisResult;

                nextCursor = long.Parse((string)innerResult[0]);

                var resultLines = ((string[])innerResult[1]).ToArray();
                keys.UnionWith(resultLines);
            } while (nextCursor != 0);

            return keys;
        }

        public async static Task<bool> UpdateHashAsync(this IDatabase database, string docId, Dictionary<string, dynamic> docDic)
        {
            List<HashEntry> addedFields = new();
            List<RedisValue> deletedFields = new();

            foreach (var item in docDic)
            {
                if (item.Value is null)
                {
                    deletedFields.Add(item.Key);
                }
                else
                {
                    addedFields.Add(new HashEntry(item.Key, RedisValue.Unbox(item.Value)));
                }
            }

            var trn = database.CreateTransaction();

            _ = trn.HashSetAsync(docId, addedFields.ToArray());

            _ = trn.HashDeleteAsync(docId, deletedFields.ToArray());

            return await trn.ExecuteAsync();
        }

        public async static Task BulkUpdate(this IDatabase database, string pattern, Dictionary<string, dynamic> dict)
        {
            var docIds = await database.ScanKeys(pattern);
            await database.BulkUpdate(docIds, dict);
        }

        public async static Task BulkUpdate(this IDatabase database, IEnumerable<string> docIds, Dictionary<string, dynamic> dict)
        {
            var trn = database.CreateTransaction();

            var entries = new List<HashEntry>();

            foreach (var item in dict)
            {
                entries.Add(new HashEntry(item.Key, item.Value));
            }

            foreach (var docId in docIds)
            {
                _ = trn.HashSetAsync(docId, entries.ToArray());
            }

            await trn.ExecuteAsync();
        }

        public async static Task<bool> BulkDelete(this IDatabase database, List<string> docIds)
        {
            var trn = database.CreateTransaction();

            foreach (var docId in docIds)
            {
                _ = trn.KeyDeleteAsync(docId);
            }

            return await trn.ExecuteAsync();
        }

        private static readonly char[] charSet = { '/', ' ', ',', '.', '<', '>', '{', '}', '[', ']', '"', '\'', ':', ';', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '+', '=', '~' };

        private static ReadOnlySpan<char> Buffer => charSet;

        public static string RsLiteral(this string input)
        {
            var literal = new StringBuilder();

            foreach (var c in input)
            {
                if (Buffer.Contains(c)) literal.Append('\\');
                literal.Append(c);
            }

            return literal.ToString();
        }

        public static async Task<long> Message<T>(this IDatabase database, T evt) where T : IMessage
        {
            var payload = JsonSerializer.Serialize(evt);
            return await database.PublishAsync(evt.GetId(), payload);
        }

        public static async Task<string> Notify<T>(this IDatabase database, T evt, string userId = "BHM:SYSTEM") where T : INotify
        {
            var type = evt.GetType().ToString();
            var payload = JsonSerializer.Serialize(evt, evt.GetType());

            var entries = new[]
            {
                new NameValueEntry("type", type),
                new NameValueEntry("payload", payload),
                new NameValueEntry("userId", userId)
            };

            return await database.StreamAddAsync(evt.GetKey(), entries);
        }
    }
}
