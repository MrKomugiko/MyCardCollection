using Microsoft.Extensions.Caching.Memory;

namespace MyCardCollection.Repository
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private MemoryCacheEntryOptions _cacheExpiryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddSeconds(3600),
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromSeconds(3600)
        };

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public bool TryGetValue<TItem>(string key, out TItem value)
        {
            if(_memoryCache.TryGetValue(key, out value))
            {
                return true;
            }
            return false;

        }

        public void Set(string key, object value)
        {
            _memoryCache.Set(key, value, _cacheExpiryOptions);
        }

        public void Remove(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }

        public TItem Get<TItem>(string key)
        {
            TryGetValue<TItem>(key, out var value);
            return value;
        }

    }
}
