using Itau.Transfer.Application.Interfaces;
using System.Text.Json;
using System.Text;

namespace Itau.Transfer.Infrastructure.HttpClient;

public class HttpClientHelper(IHttpClientFactory clientFactory) : IHttpClientHelper
{
    public async Task<T> GetAsync<T>(string clientName, string path)
    {
        var httpClient = clientFactory.CreateClient(clientName);

        var response = await httpClient.GetAsync(path);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string clientName, string path, TRequest body, CancellationToken ct)
    {
        var httpClient = clientFactory.CreateClient(clientName);
 
        var jsonContent = JsonSerializer.Serialize(body);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(path, httpContent, ct);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TResponse>(responseContent);
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string clientName, string path, TRequest body, CancellationToken ct)
    {
        var httpClient = clientFactory.CreateClient(clientName);

        var jsonContent = JsonSerializer.Serialize(body, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower

        });
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await httpClient.PutAsync(path, httpContent, ct);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<TResponse>(responseContent);
    }
     
}