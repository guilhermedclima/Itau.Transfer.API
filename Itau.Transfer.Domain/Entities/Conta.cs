namespace Itau.Transfer.Domain.Entities;
public class Conta
{
    public Guid Id { get; set; }
    public decimal Saldo { get; set; }
    public bool Ativo { get; set; }
    public decimal LimiteDiario { get; set; }

    public Conta()
    {
    }

    public Conta(Guid id, decimal saldo, bool ativo, decimal limiteDiario)
    {
        Id = id;
        Saldo = saldo;
        Ativo = ativo;
        LimiteDiario = limiteDiario;
    }
}
