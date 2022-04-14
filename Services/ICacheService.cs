namespace MyCardCollection.Repository
{
    public interface ICacheService
    {
        void Set(string key, object value);
        bool TryGetValue<TItem>(string key, out TItem value);
        void Remove(string cacheKey);
        TItem Get<TItem>(string key);
    }
}