using gateway_api.Models.Weather;

namespace gateway_api.Clients;

public class WeatherHttpClient
{
    private readonly HttpClient _client;
    private readonly ILogger<WeatherHttpClient> _logger;

    public WeatherHttpClient(HttpClient client, ILogger<WeatherHttpClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<GetWeather.Response?> GetWeather(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "weather/get");
        var response = await _client.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetWeather.Response>(cancellationToken: cancellationToken);

        return data;
    }
}