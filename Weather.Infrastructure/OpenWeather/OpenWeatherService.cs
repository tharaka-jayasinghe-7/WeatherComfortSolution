using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Weather.Domain.Interfaces;
using Weather.Domain.ValueObjects;

namespace Weather.Infrastructure.OpenWeather;

public class OpenWeatherService : IWeatherProvider
{
    private readonly HttpClient _httpClient;
    private readonly ICacheService _cache;
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public OpenWeatherService(
        HttpClient httpClient,
        ICacheService cache,
        IConfiguration config)
    {
        _httpClient = httpClient;
        _cache = cache;
        _apiKey = config["OpenWeather:ApiKey"] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new ArgumentException("OpenWeather:ApiKey configuration value is missing or empty.", nameof(config));
    }

    public async Task<WeatherMetrics> GetWeatherAsync(string cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName))
            throw new ArgumentException("cityName must be provided", nameof(cityName));

        var cityKey = cityName.ToLowerInvariant();

        // Check processed metrics cache first
        var metricsCacheKey = $"weather:metrics:{cityKey}";
        var cachedMetrics = await _cache.GetAsync<WeatherMetrics>(metricsCacheKey);
        if (cachedMetrics is not null)
        {
            Console.WriteLine("Cache HIT (processed metrics)");
            return cachedMetrics;
        }
        Console.WriteLine("Cache MISS (processed metrics)");

        // Check raw API response cache
        var rawCacheKey = $"weather:raw:{cityKey}";
        var cachedRawJson = await _cache.GetAsync<string>(rawCacheKey);
        OpenWeatherResponse data;

        if (cachedRawJson is not null)
        {
            Console.WriteLine("Cache HIT (raw API)");
            data = JsonSerializer.Deserialize<OpenWeatherResponse>(cachedRawJson, _jsonOptions)!;
        }
        else
        {
            Console.WriteLine("Cache MISS (raw API)");
            var url = $"weather?q={Uri.EscapeDataString(cityName)}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenWeather API returned {(int)response.StatusCode} {response.ReasonPhrase} for city '{cityName}'. Response body: {body}");
            }

            var json = await response.Content.ReadAsStringAsync();

            data = JsonSerializer.Deserialize<OpenWeatherResponse>(json, _jsonOptions)
                ?? throw new InvalidOperationException($"Failed to deserialize OpenWeather response for '{cityName}'. Raw body: {json}");

            // Cache raw response for 5 minutes
            await _cache.SetAsync(rawCacheKey, json, TimeSpan.FromMinutes(5));
        }

        if (data.Main is null || data.Wind is null || data.Clouds is null)
            throw new InvalidOperationException($"OpenWeather response for '{cityName}' is missing expected fields.");

        var metrics = new WeatherMetrics
        {
            TemperatureCelsius = data.Main.Temp,
            Humidity = data.Main.Humidity,
            WindSpeed = data.Wind.Speed,
            Cloudiness = data.Clouds.All
        };

        // Cache processed metrics separately for 5 minutes
        await _cache.SetAsync(metricsCacheKey, metrics, TimeSpan.FromMinutes(5));

        return metrics;
    }

    // Optional debug method to check cache status
    public async Task<(bool MetricsHit, bool RawHit)> CheckCacheStatusAsync(string cityName)
    {
        var cityKey = cityName.ToLowerInvariant();
        var metricsHit = await _cache.GetAsync<WeatherMetrics>($"weather:metrics:{cityKey}") is not null;
        var rawHit = await _cache.GetAsync<string>($"weather:raw:{cityKey}") is not null;
        return (metricsHit, rawHit);
    }
}
