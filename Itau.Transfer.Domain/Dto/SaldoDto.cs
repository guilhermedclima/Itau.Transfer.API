namespace Itau.Transfer.Domain.Dto;

public class SaldoDto
{
    public decimal Valor { get; set; }
    public TransferenciaContaDto Conta { get; set; }
}