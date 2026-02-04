using System;
using System.Threading.Tasks;
using System.Text.Json;
using StackExchange.Redis;
using Weather.Domain.Interfaces;

namespace Weather.Infrastructure.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key).ConfigureAwait(false);
        if (value.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        // Use StringSetAsync without expiry and apply expiry with KeyExpireAsync to avoid Expiration/FromTimeSpan differences
        await _db.StringSetAsync(key, json).ConfigureAwait(false);
        if (expiry.HasValue)
        {
            await _db.KeyExpireAsync(key, expiry).ConfigureAwait(false);
        }
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key).ConfigureAwait(false);
    }
}