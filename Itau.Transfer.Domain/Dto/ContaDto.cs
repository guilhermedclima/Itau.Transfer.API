namespace Itau.Transfer.Domain.Dto;

public class ContaDto
{
    public Guid Id { get; set; }
    public decimal Saldo { get; set; }
    public bool Ativo { get; set; }
    public decimal LimiteDiario { get; set; }
}