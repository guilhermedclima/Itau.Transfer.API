using Itau.Transfer.Domain.Dto;

namespace Itau.Transfer.Application.Interfaces.Services;

public interface IContaService
{
    Task<ContaDto> GetContaAsync(Guid id);

    Task AtualizarSaldoAsync(SaldoDto saldo, CancellationToken ct);
}