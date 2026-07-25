namespace Application;

public interface IRedisCacheService
{
    Task SetRecordAsync<T>(string key, T value, TimeSpan expiry);
    Task<T?> GetRecordAsync<T>(string key);
    Task RemoveRecordAsync(string key);
    Task RemoveManyAsync(params string[] keys);
}
