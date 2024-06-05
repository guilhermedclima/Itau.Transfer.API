namespace Itau.Transfer.Domain.Entities;

public class Transferencia
{
    public Guid Id { get; set; }
    public Guid IdCliente { get; set; }
    public decimal Valor { get; set; }
    public TransferenciaContas TransferenciaContas { get; set; }
}