using Itau.Transfer.Domain.Entities;
using Itau.Transfer.Infrastructure.Context;
using Itau.Transfer.Infrastructure.Interfaces.Repositories;

namespace Itau.Transfer.Infrastructure.Repositories;

public class TransferenciaRepository(AppDbContext context) : ITransferenciaRepository
{
    public async Task InserirTransferenciaAsync(Transferencia transferencia)
    {
        await context.Transferencias.AddAsync(transferencia);
    }
}