using Weather.Application.DTOs;
using Weather.Domain.Interfaces;

namespace Weather.Application.UseCases;

public class GetCityComfortRanking
{
    private readonly IWeatherProvider _weatherProvider;
    private readonly IComfortIndexCalculator _calculator;

    public GetCityComfortRanking(
        IWeatherProvider weatherProvider,
        IComfortIndexCalculator calculator)
    {
        _weatherProvider = weatherProvider;
        _calculator = calculator;
    }

    public async Task<List<CityComfortDto>> ExecuteAsync(IEnumerable<string> cities)
    {
        var results = new List<CityComfortDto>();

        foreach (var city in cities)
        {
            var weather = await _weatherProvider.GetWeatherAsync(city);
            var comfort = _calculator.Calculate(weather);

            results.Add(new CityComfortDto
            {
                City = city,
                ComfortScore = comfort.Value,
                Description = comfort.Description,
                Weather = new WeatherDto
                {
                    Temperature = weather.TemperatureCelsius,
                    Humidity = weather.Humidity,
                    WindSpeed = weather.WindSpeed,
                    Cloudiness = weather.Cloudiness
                }
            });
        }

        return results
            .OrderByDescending(x => x.ComfortScore)
            .ToList();
    }
}