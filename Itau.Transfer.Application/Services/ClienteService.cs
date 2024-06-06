using Itau.Transfer.Application.Interfaces.Services;
using Itau.Transfer.Domain.Dto;
using Itau.Transfer.Infrastructure.Interfaces.Helpers;
using Microsoft.Extensions.Logging;

namespace Itau.Transfer.Application.Services;

public class ClienteService(IHttpClientHelper clientHelper, ILogger<ClienteService> logger) : IClienteService
{
    public async Task<ClienteDto?> GetClienteAsync(Guid id)
    {
        logger.LogInformation($"Obtendo Cliente {id}");
        return await clientHelper.GetAsync<ClienteDto>("ClientesEContasApi", $"clientes/{id}");
    }
}