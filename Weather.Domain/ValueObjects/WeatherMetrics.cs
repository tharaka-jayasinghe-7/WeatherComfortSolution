namespace Weather.Domain.ValueObjects;

public class WeatherMetrics
{
    public double TemperatureCelsius { get; init; }
    public int Humidity { get; init; }
    public double WindSpeed { get; init; }
    public int Cloudiness { get; init; }
}