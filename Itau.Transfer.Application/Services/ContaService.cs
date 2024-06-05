using Itau.Transfer.Application.Interfaces.Services;
using Itau.Transfer.Domain.Dto;
using Itau.Transfer.Domain.Entities;
using Itau.Transfer.Infrastructure.Interfaces.Helpers;
using Microsoft.Extensions.Logging;

namespace Itau.Transfer.Application.Services;

public class ContaService(IHttpClientHelper clientHelper, ILogger<ContaService> logger) : IContaService
{
    public async Task<Conta> GetContaAsync(Guid id)
    {
        logger.LogInformation($"Getting Conta {id}");
        return await clientHelper.GetAsync<Conta>("ClientesEContasApi", $"contas/{id}");
    }

    public async Task AtualizarSaldoAsync(SaldoDto saldo, CancellationToken ct)
    {
        logger.LogInformation($"Atualizando saldo da conta destino {saldo.Conta.IdDestino} e da conta origem {saldo.Conta.IdOrigem} valor {saldo.Valor}");
        await clientHelper.PutAsync("ClientesEContasApi", $"contas/saldos", saldo, ct);
    }
}