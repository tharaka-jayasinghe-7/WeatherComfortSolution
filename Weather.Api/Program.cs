using StackExchange.Redis;
using Weather.Application.Services;
using Weather.Application.UseCases;
using Weather.Domain.Interfaces;
using Weather.Infrastructure.Cache;
using Weather.Infrastructure.OpenWeather;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache(); // for IMemoryCache

// HttpClient for OpenWeather
builder.Services.AddHttpClient<IWeatherProvider, OpenWeatherService>(client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
});

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]!));

// Application services
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IComfortIndexCalculator, ComfortIndexCalculator>();
builder.Services.AddScoped<GetCityComfortRanking>();

// ------------------------
// CORS configuration
// ------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // your React app URL
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Enable CORS
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();