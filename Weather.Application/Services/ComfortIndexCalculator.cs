using Weather.Domain.Interfaces;
using Weather.Domain.ValueObjects;

namespace Weather.Application.Services;

public class ComfortIndexCalculator : IComfortIndexCalculator
{
    public ComfortScore Calculate(WeatherMetrics metrics)
    {
        double tempScore = 100 - Math.Abs(metrics.TemperatureCelsius - 22) * 3;
        double humidityScore = 100 - Math.Abs(metrics.Humidity - 50) * 1.5;
        double windScore = 100 - metrics.WindSpeed * 5;
        double cloudScore = 100 - metrics.Cloudiness * 0.5;

        double score =
            (tempScore * 0.4) +
            (humidityScore * 0.25) +
            (windScore * 0.2) +
            (cloudScore * 0.15);

        score = Math.Clamp(score, 0, 100);

        return new ComfortScore
        {
            Value = Math.Round(score, 2),
            Description = score >= 80 ? "Very Comfortable" :
                score >= 60 ? "Comfortable" :
                score >= 40 ? "Moderate" : "Uncomfortable"
        };
    }
}