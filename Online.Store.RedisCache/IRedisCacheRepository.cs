using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Online.Store.RedisCache
{
    public interface IRedisCacheRepository
    {
        Task SetStringAsync(string key, string value);
        Task SetStringAsync(string key, string value, int expirationMinutes);

        Task SetItemAsync(string key, object item);
        Task SetItemAsync(string key, object item, int expirationMinutes);

        Task<T> GetItemAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
