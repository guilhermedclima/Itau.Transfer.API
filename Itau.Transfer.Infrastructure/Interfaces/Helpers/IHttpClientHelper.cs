namespace Itau.Transfer.Infrastructure.Interfaces.Helpers;

public interface IHttpClientHelper
{
    Task<T?> GetAsync<T>(string clientName, string path);

    Task PostAsync<T>(string clientName, string path, T body, CancellationToken ct);

    Task PutAsync<T>(string clientName, string path, T body, CancellationToken ct);
}