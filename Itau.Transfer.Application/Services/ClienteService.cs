using Itau.Transfer.Application.Interfaces.Services;
using Itau.Transfer.Domain.Entities;
using Itau.Transfer.Infrastructure.Interfaces.Helpers;
using Microsoft.Extensions.Logging;

namespace Itau.Transfer.Application.Services;

public class ClienteService(IHttpClientHelper clientHelper, ILogger<ClienteService> logger): IClienteService
{
    public async Task<Cliente> GetClienteAsync(Guid id)
    {
        logger.LogInformation($"Getting Cliente {id}");
        return await clientHelper.GetAsync<Cliente>("ClientesEContasApi", $"clientes/{id}");
    }
}