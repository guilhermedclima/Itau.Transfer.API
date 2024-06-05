using Itau.Transfer.Domain.Dto;

namespace Itau.Transfer.Domain.Entities;
public class Transferencia
{
    public Guid Id { get; set; }
    public Guid IdCliente { get; set; }
    public decimal Valor { get; set; }
    public TransferenciaContaDto Conta { get; set; }

    public Transferencia()
    {
    }

    public Transferencia(Guid idCliente, decimal valor, TransferenciaContaDto conta)
    {
        IdCliente = idCliente;
        Conta = conta;
    }
}
