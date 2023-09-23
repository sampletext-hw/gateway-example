using gateway_api.Models.Salary;
using gateway_api.Models.Weather;

namespace gateway_api.Clients;

public class SalaryHttpClient
{
    private readonly HttpClient _client;
    private readonly ILogger<SalaryHttpClient> _logger;

    public SalaryHttpClient(HttpClient client, ILogger<SalaryHttpClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<GetSalary.Response?> GetSalary(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "salary/get");
        var response = await _client.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetSalary.Response>(cancellationToken: cancellationToken);

        return data;
    }
}