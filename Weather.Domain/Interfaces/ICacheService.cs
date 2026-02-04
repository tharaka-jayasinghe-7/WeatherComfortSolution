// filepath: f:\NIBM\Projects 2025\WeatherComfortSolution\Weather.Domain\Interfaces\ICacheService.cs
using System;
using System.Threading.Tasks;

namespace Weather.Domain.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
}
