namespace Itau.Transfer.Domain.Dto;

public class ContaDto
{
    public Guid Id { get; set; }
    public decimal Saldo { get; set; }
    public bool Ativo { get; set; }
    public decimal LimiteDiario { get; set; }

    public ContaDto()
    {
    }

    public ContaDto(Guid id, decimal saldo, bool ativo, decimal limiteDiario)
    {
        Id = id;
        Saldo = saldo;
        Ativo = ativo;
        LimiteDiario = limiteDiario;
    }
}