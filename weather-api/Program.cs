using weather_api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var app = builder.Build();

var cities = new[] {"Moscow", "London", "Brooklyn", "Rome", "New-York", "Paris", "Madrid", "Saint-Petersburg"};

app.MapGet(
    "/weather/get",
    () => new GetWeather.Response(
        Enumerable.Range(0, 5)
            .Select(x => new GetWeather.Item(cities[Random.Shared.Next(cities.Length)], DateTime.UtcNow, Random.Shared.NextSingle() * 30))
    )
);

app.UseHealthChecks("/health-check");

app.Run();