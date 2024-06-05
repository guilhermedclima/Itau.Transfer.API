using Itau.Transfer.Domain.Dto;
using Itau.Transfer.Domain.Entities;

namespace Itau.Transfer.Application.Interfaces.Services;

public interface IContaService
{
    Task<Conta> GetContaAsync(Guid id);
    Task AtualizarSaldoAsync(SaldoDto saldo, CancellationToken ct);
}