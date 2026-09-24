using Microsoft.Extensions.Caching.Distributed;
using SharedKernel.Interfaces;
using System.Text.Json;

namespace SharedKernel.Services
{
    public class Cache : ICache
    {

        private readonly IDistributedCache _cache;

        public Cache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var json = await _cache.GetStringAsync(key, cancellationToken);
            if (json == null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(json);

        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expirastion, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirastion,
            };
            await _cache.SetStringAsync(key, json, options, cancellationToken);

        }
    }
}
