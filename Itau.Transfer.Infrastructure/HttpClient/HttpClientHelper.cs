using Itau.Transfer.Domain.Exception;
using Itau.Transfer.Infrastructure.Interfaces.Helpers;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Itau.Transfer.Infrastructure.HttpClient;

public class HttpClientHelper(IHttpClientFactory clientFactory, ILogger<HttpClientHelper> logger) : IHttpClientHelper
{
    public async Task<T?> GetAsync<T>(string clientName, string path)
    {
        try
        {
            logger.LogInformation($"Requisitando {path} do {clientName} client");
            var httpClient = clientFactory.CreateClient(clientName);

            using var response = await httpClient.GetAsync(path);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return default(T);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<T>(content, options);
        }
        catch (HttpRequestException e)
        {
            logger.LogError(e, e.Message);
            throw new HttpClientRequestException(e.Message);
        }
    }

    public async Task PostAsync<T>(string clientName, string path, T body, CancellationToken ct)
    {
        try
        {
            logger.LogInformation($"Requisitando {path} do {clientName} client");
            var httpClient = clientFactory.CreateClient(clientName);
            var jsonContent = JsonSerializer.Serialize(body);
            using var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(path, httpContent, ct);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync(ct);
        }
        catch (HttpRequestException e)
        {
            logger.LogError(e, e.Message);
            throw new HttpClientRequestException(e.Message);
        }
    }

    public async Task PutAsync<T>(string clientName, string path, T body, CancellationToken ct)
    {
        try
        {
            logger.LogInformation($"Requisitando {path} do {clientName} client");
            var httpClient = clientFactory.CreateClient(clientName);
            var jsonContent = JsonSerializer.Serialize(body, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });
            using var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync(path, httpContent, ct);
            response.EnsureSuccessStatusCode();

            await response.Content.ReadAsStringAsync(ct);
        }
        catch (HttpRequestException e)
        {
            logger.LogError(e, e.Message);
            throw new HttpClientRequestException(e.Message);
        }
    }
}