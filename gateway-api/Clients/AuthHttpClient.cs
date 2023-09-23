using System.Net;
using gateway_api.Models.Auth;
using gateway_api.Models.Salary;
using gateway_api.Models.Weather;

namespace gateway_api.Clients;

public class AuthHttpClient
{
    private readonly HttpClient _client;
    private readonly ILogger<AuthHttpClient> _logger;

    public AuthHttpClient(HttpClient client, ILogger<AuthHttpClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<string?> Auth(Auth.Request auth, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "auth")
        {
            Content = JsonContent.Create(auth)
        };
        var response = await _client.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }
        
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadAsStringAsync(cancellationToken);

        return data;
    }
}