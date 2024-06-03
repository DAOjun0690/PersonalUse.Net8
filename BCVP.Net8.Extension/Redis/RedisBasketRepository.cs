using BCVP.Net8.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.ComponentModel;

namespace BCVP.Net8.Extension.Redis
{
    [Description("普通快取考慮直接使用ICaching,如果要使用Redis等還是使用此類別")]
    public class RedisBasketRepository : IRedisBasketRepository
    {
        private readonly ILogger<RedisBasketRepository> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisBasketRepository(ILogger<RedisBasketRepository> logger, ConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            _database = redis.GetDatabase();
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }

        public async Task Clear()
        {
            foreach (var endPoint in _redis.GetEndPoints())
            {
                var server = GetServer();
                foreach (var key in server.Keys())
                {
                    await _database.KeyDeleteAsync(key);
                }
            }
        }

        public async Task<bool> Exist(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task<string> GetValue(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async Task Remove(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task Set(string key, object value, TimeSpan cacheTime)
        {
            if (value != null)
            {
                if (value is string cacheValue)
                {
                    // string 不需要序列化
                    await _database.StringSetAsync(key, cacheValue, cacheTime);
                }
                else
                {
                    // 序列化，將object值生成RedisValue
                    await _database.StringSetAsync(key, SerializeHelper.Serialize(value), cacheTime);
                }
            }
        }

        public async Task<TEntity> Get<TEntity>(string key)
        {
            var value = await _database.StringGetAsync(key);
            if (value.HasValue)
            {
                //需要用的反序列化，将Redis存储的Byte[]，进行反序列化
                return SerializeHelper.Deserialize<TEntity>(value);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 根據key獲得RedisValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<RedisValue[]> ListRangeAsync(string redisKey)
        {
            return await _database.ListRangeAsync(redisKey);
        }

        /// <summary>
        /// 在表格head插入值。如果key不存在，先建立在插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task<long> ListLeftPushAsync(string redisKey, string redisValue, int db = -1)
        {
            return await _database.ListLeftPushAsync(redisKey, redisValue);
        }
        /// <summary>
        /// 在表格尾部插入值。如果key不存在，先建立在插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task<long> ListRightPushAsync(string redisKey, string redisValue, int db = -1)
        {
            return await _database.ListRightPushAsync(redisKey, redisValue);
        }

        /// <summary>
        /// 在表格尾部插入數組集合。如果key不存在，先建立在插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task<long> ListRightPushAsync(string redisKey, IEnumerable<string> redisValue, int db = -1)
        {
            var redislist = new List<RedisValue>();
            foreach (var item in redisValue)
            {
                redislist.Add(item);
            }
            return await _database.ListRightPushAsync(redisKey, redislist.ToArray());
        }


        /// <summary>
        /// 移除並返回儲存在該key list的第一個元素 反序列化
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<T> ListLeftPopAsync<T>(string redisKey, int db = -1) where T : class
        {
            return JsonConvert.DeserializeObject<T>(await _database.ListLeftPopAsync(redisKey));
        }

        /// <summary>
        /// 移除並返回儲存在該key list的第一個元素 反序列化
        /// 只能是物件集合
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<T> ListRightPopAsync<T>(string redisKey, int db = -1) where T : class
        {
            return JsonConvert.DeserializeObject<T>(await _database.ListRightPopAsync(redisKey));
        }

        /// <summary>
        /// 移除並返回儲存在該Key list的第一個元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<string> ListLeftPopAsync(string redisKey, int db = -1)
        {
            return await _database.ListLeftPopAsync(redisKey);
        }

        /// <summary>
        /// 移除並返回儲存在該Key list的最後一個元素 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<string> ListRightPopAsync(string redisKey, int db = -1)
        {
            return await _database.ListRightPopAsync(redisKey);
        }

        /// <summary>
        /// list長度
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string redisKey, int db = -1)
        {
            return await _database.ListLengthAsync(redisKey);
        }

        /// <summary>
        /// 返回在該list上key所對應的元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> ListRangeAsync(string redisKey, int db = -1)
        {
            var result = await _database.ListRangeAsync(redisKey);
            return result.Select(o => o.ToString());
        }

        /// <summary>
        /// 根據index獲得指定位置資料
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> ListRangeAsync(string redisKey, int start, int stop, int db = -1)
        {
            var result = await _database.ListRangeAsync(redisKey, start, stop);
            return result.Select(o => o.ToString());
        }

        /// <summary>
        /// 刪除List中的元素，並返回刪除的元素的數量
        /// </summary>
        /// <param name="redisKey">key</param>
        /// <param name="redisValue">元素</param>
        /// <param name="type">大于零 : 从表头开始向表尾搜索，小于零 : 从表尾开始向表头搜索，等于零：移除表中所有与 VALUE 相等的值</param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<long> ListDelRangeAsync(string redisKey, string redisValue, long type = 0, int db = -1)
        {
            return await _database.ListRemoveAsync(redisKey, redisValue, type);
        }

        /// <summary>
        /// 清空List
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        public async Task ListClearAsync(string redisKey, int db = -1)
        {
            await _database.ListTrimAsync(redisKey, 1, 0);
        }
    }
}
