namespace Weather.Application.DTOs;

public class CityComfortDto
{
    public string City { get; set; } = default!;
    public double ComfortScore { get; set; }
    public string Description { get; set; } = default!;
    public WeatherDto Weather { get; set; } = default!;
}