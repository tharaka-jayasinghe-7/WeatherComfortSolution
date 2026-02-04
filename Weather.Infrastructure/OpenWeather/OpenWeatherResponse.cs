using System.Text.Json.Serialization;

namespace Weather.Infrastructure.OpenWeather;

public class OpenWeatherResponse
{
    public Main Main { get; set; } = default!;
    public Wind Wind { get; set; } = default!;
    public Clouds Clouds { get; set; } = default!;
}

public class Main
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public class Wind
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}

public class Clouds
{
    [JsonPropertyName("all")]
    public int All { get; set; }
}