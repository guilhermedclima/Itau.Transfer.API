using Itau.Transfer.Domain.Dto;
using Itau.Transfer.Domain.Entities;

namespace Itau.Transfer.Application.Interfaces.Services;

public interface ITransferenciaService
{
    Task<TransferenciaResponseDto> TransferenciaAsync(Transferencia request, CancellationToken ct);
}