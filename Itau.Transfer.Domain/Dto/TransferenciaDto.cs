namespace Itau.Transfer.Domain.Dto;

public class TransferenciaDto
{
    public Guid Id { get; set; }
    public Guid IdCliente { get; set; }
    public decimal Valor { get; set; }
    public TransferenciaContaDto Conta { get; set; }

}