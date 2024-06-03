namespace Itau.Transfer.Application.Interfaces;

public interface IHttpClientHelper
{
    Task<T> GetAsync<T>(string clientName, string path);
    Task<TResponse> PostAsync<TRequest, TResponse>(string clientName, string path, TRequest body, CancellationToken ct);
    Task<TResponse> PutAsync<TRequest, TResponse>(string clientName, string path, TRequest body, CancellationToken ct);
}