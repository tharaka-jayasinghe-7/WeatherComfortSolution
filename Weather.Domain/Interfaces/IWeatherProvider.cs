using Weather.Domain.ValueObjects;

namespace Weather.Domain.Interfaces;

public interface IWeatherProvider
{
    Task<WeatherMetrics> GetWeatherAsync(string cityName);
}