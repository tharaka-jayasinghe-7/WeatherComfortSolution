using Microsoft.AspNetCore.Mvc;
using Weather.Application.UseCases;

namespace Weather.Api.Controllers;

[ApiController]
[Route("api/weather")]
public class WeatherController : ControllerBase
{
    private readonly GetCityComfortRanking _useCase;

    public WeatherController(GetCityComfortRanking useCase)
    {
        _useCase = useCase;
    }

    [HttpGet("comfort")]
    public async Task<IActionResult> GetComfort()
    {
        var cities = new[]
        {
            "Colombo",
            "Singapore",
            "Bangkok",
            "Tokyo",
            "Sydney"
        };

        var result = await _useCase.ExecuteAsync(cities);
        return Ok(result);
    }
}