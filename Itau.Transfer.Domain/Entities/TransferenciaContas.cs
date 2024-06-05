namespace Itau.Transfer.Domain.Entities;

public class TransferenciaContas
{
    public Guid Id { get; set; }
    public Guid IdOrigem { get; set; }
    public Guid IdDestino { get; set; }
}