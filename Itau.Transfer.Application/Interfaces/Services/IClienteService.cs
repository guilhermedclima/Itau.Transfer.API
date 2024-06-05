using Itau.Transfer.Domain.Entities;

namespace Itau.Transfer.Application.Interfaces.Services;

public interface IClienteService
{
    Task<Cliente> GetClienteAsync(Guid id);
}