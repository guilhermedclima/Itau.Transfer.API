using Itau.Transfer.Domain.Entities;

namespace Itau.Transfer.Infrastructure.Interfaces.Repositories;

public interface ITransferenciaRepository
{
    Task InserirTransferenciaAsync(Transferencia transferencia);
}