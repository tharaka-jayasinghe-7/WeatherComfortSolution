using FluentAssertions;
using Weather.Application.Services;
using Weather.Domain.ValueObjects;
using Xunit;

namespace Weather.Application.Tests;

public class ComfortIndexCalculatorTests
{
    private readonly ComfortIndexCalculator _calculator = new();

    [Fact]
    public void Calculate_Should_Return_VeryComfortable_For_Ideal_Weather()
    {
        var metrics = new WeatherMetrics
        {
            TemperatureCelsius = 22,
            Humidity = 50,
            WindSpeed = 1,
            Cloudiness = 10
        };

        var result = _calculator.Calculate(metrics);

        result.Value.Should().BeGreaterThan(80.0);
        result.Description.Should().Be("Very Comfortable");
    }

    [Fact]
    public void Calculate_Should_Return_Uncomfortable_When_Score_Is_Below_40()
    {
        var metrics = new WeatherMetrics
        {
            TemperatureCelsius = 65,
            Humidity = 100,
            WindSpeed = 40,
            Cloudiness = 100
        };

        var result = _calculator.Calculate(metrics);

        result.Value.Should().BeLessThan(40.0);
        result.Description.Should().Be("Uncomfortable");
    }

    [Fact]
    public void Calculate_Should_Return_Moderate_When_Score_Is_Between_40_And_59()
    {
        var metrics = new WeatherMetrics
        {
            TemperatureCelsius = 38,
            Humidity = 85,
            WindSpeed = 10,
            Cloudiness = 80
        };

        var result = _calculator.Calculate(metrics);

        result.Value.Should().BeInRange(40.0, 59.99);
        result.Description.Should().Be("Moderate");
    }


    [Fact]
    public void Calculate_Should_Return_Comfortable_When_Score_Is_Between_60_And_79()
    {
        var metrics = new WeatherMetrics
        {
            TemperatureCelsius = 30,
            Humidity = 70,
            WindSpeed = 4,
            Cloudiness = 40
        };

        var result = _calculator.Calculate(metrics);

        result.Value.Should().BeInRange(60.0, 79.99);
        result.Description.Should().Be("Comfortable");
    }

    [Fact]
    public void Calculate_Should_Clamp_Score_Between_0_And_100()
    {
        var metrics = new WeatherMetrics
        {
            TemperatureCelsius = -100,
            Humidity = 0,
            WindSpeed = 100,
            Cloudiness = 100
        };

        var result = _calculator.Calculate(metrics);

        result.Value.Should().BeGreaterThanOrEqualTo(0.0);
        result.Value.Should().BeLessThanOrEqualTo(100.0);
    }
}
