using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.RedisCache
{
    public class RedisCacheReposistory : IRedisCacheRepository
    {
        private readonly IDistributedCache _distributedCache;
        private int _defaultExpirationMinutes;

        public RedisCacheReposistory(IDistributedCache distributedCache, IConfiguration configuration)
        {
            this._distributedCache = distributedCache;
            this._defaultExpirationMinutes = Int32.Parse(configuration["RedisCache:DefaultExpirationMinutes"]);
        }

        public async Task SetStringAsync(string key, string value)
        {
            await SetStringAsync(key, value, this._defaultExpirationMinutes);
        }

        public async Task SetStringAsync(string key, string value, int expirationMinutes)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(expirationMinutes)
            };

            await _distributedCache.SetAsync(key, Encoding.UTF8.GetBytes(value), options);
        }

        public async Task SetItemAsync(string key, object item)
        {
            string json = JsonConvert.SerializeObject(item);

            await SetStringAsync(key, json);
        }

        public async Task SetItemAsync(string key, object item, int expirationMinutes)
        {
            string json = JsonConvert.SerializeObject(item);

            await SetStringAsync(key, json, expirationMinutes);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            string json = await _distributedCache.GetStringAsync(key);

            if (json == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
