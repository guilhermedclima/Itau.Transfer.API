using Itau.Transfer.Domain.Dto;

namespace Itau.Transfer.Application.Interfaces.Services;

public interface IClienteService
{
    Task<ClienteDto> GetClienteAsync(Guid id);
}