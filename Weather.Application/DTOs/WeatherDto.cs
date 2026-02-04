namespace Weather.Application.DTOs;

public class WeatherDto
{
    public double Temperature { get; set; }
    public int Humidity { get; set; }
    public double WindSpeed { get; set; }
    public int Cloudiness { get; set; }
}