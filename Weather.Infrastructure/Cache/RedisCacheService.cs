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
    private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(5); 

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key).ConfigureAwait(false);
        if (value.IsNullOrEmpty)
        {
            Console.WriteLine($"Cache MISS for key: {key}");
            return default;
        }

        Console.WriteLine($"Cache HIT for key: {key}");
        return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);

        // Use default expiry if none is provided
        var effectiveExpiry = expiry ?? _defaultExpiry;

        await _db.StringSetAsync(key, json, effectiveExpiry).ConfigureAwait(false);
        Console.WriteLine($"Cache SET for key: {key} with expiry: {effectiveExpiry.TotalMinutes} min");
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key).ConfigureAwait(false);
        Console.WriteLine($"Cache REMOVED for key: {key}");
    }
}