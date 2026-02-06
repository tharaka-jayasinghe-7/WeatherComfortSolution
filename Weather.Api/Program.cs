using StackExchange.Redis;
using Weather.Application.Services;
using Weather.Application.UseCases;
using Weather.Domain.Interfaces;
using Weather.Infrastructure.Cache;
using Weather.Infrastructure.OpenWeather;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Register in-memory cache used by some services (resolves IMemoryCache DI error)
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IWeatherProvider, OpenWeatherService>(client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        builder.Configuration["Redis:ConnectionString"]!));

builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IComfortIndexCalculator, ComfortIndexCalculator>();
builder.Services.AddScoped<GetCityComfortRanking>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowReact");

app.MapControllers();
app.Run();