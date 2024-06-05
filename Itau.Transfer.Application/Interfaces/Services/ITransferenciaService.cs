using Itau.Transfer.Domain.Dto;

namespace Itau.Transfer.Application.Interfaces.Services;

public interface ITransferenciaService
{
    Task<TransferenciaResponseDto> TransferenciaAsync(TransferenciaDto request, CancellationToken ct);
}