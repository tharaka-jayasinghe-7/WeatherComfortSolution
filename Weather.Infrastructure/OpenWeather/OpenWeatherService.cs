using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Weather.Domain.Interfaces;
using Weather.Domain.ValueObjects;

namespace Weather.Infrastructure.OpenWeather;

public class OpenWeatherService : IWeatherProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public OpenWeatherService(
        HttpClient httpClient,
        IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["OpenWeather:ApiKey"] ?? string.Empty;

        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new ArgumentException("OpenWeather:ApiKey configuration value is missing or empty.", nameof(config));
    }

    public async Task<WeatherMetrics> GetWeatherAsync(string cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName))
            throw new ArgumentException("cityName must be provided", nameof(cityName));

        // Build request URL and encode city name
        var url = $"weather?q={Uri.EscapeDataString(cityName)}&appid={_apiKey}&units=metric";

        var response = await _httpClient.GetAsync(url);

        // Throw if non-success and include status code for easier diagnosis
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"OpenWeather API returned {(int)response.StatusCode} {response.ReasonPhrase} for city '{cityName}'. Response body: {body}");
        }

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<OpenWeatherResponse>(json, _jsonOptions);

        if (data is null)
            throw new InvalidOperationException($"Failed to deserialize OpenWeather response for '{cityName}'. Raw body: {json}");

        if (data.Main is null || data.Wind is null || data.Clouds is null)
            throw new InvalidOperationException($"OpenWeather response for '{cityName}' is missing expected fields. Raw body: {json}");

        var metrics = new WeatherMetrics
        {
            TemperatureCelsius = data.Main.Temp,
            Humidity = data.Main.Humidity,
            WindSpeed = data.Wind.Speed,
            Cloudiness = data.Clouds.All
        };


        return metrics;
    }
}