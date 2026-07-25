namespace Infrastructure;

[RegisterService(typeof(IRedisCacheService), ServiceLifetime.Singleton)]
public sealed class RedisCacheService(IConnectionMultiplexer multiplexer) : IRedisCacheService
{
    private static readonly System.Text.Json.JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IDatabase _db = multiplexer.GetDatabase();

    public async Task SetRecordAsync<T>(string key, T value, TimeSpan expiry)
    {
        var data = value is string s ? s : JsonSerializer.Serialize(value, JsonOpts);
        await _db.StringSetAsync(key, data, expiry);
    }

    public async Task<T?> GetRecordAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (!value.HasValue) return default;

        if (typeof(T) == typeof(string))
            return (T)(object)(string)value!;

        return JsonSerializer.Deserialize<T>((string)value!, JsonOpts);
    }

    public async Task RemoveRecordAsync(string key) => await _db.KeyDeleteAsync(key);

    public async Task RemoveManyAsync(params string[] keys)
    {
        if (keys.Length == 0) return;
        await _db.KeyDeleteAsync(keys.Select(k => (RedisKey)k).ToArray());
    }
}
