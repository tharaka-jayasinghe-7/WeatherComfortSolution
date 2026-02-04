using Weather.Domain.ValueObjects;

namespace Weather.Domain.Interfaces;

public interface IComfortIndexCalculator
{
    ComfortScore Calculate(WeatherMetrics metrics);
}