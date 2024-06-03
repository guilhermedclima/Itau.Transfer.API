namespace Itau.Transfer.Domain.Entities;
public class Saldo
{
    public decimal Valor { get; set; }
    public Conta Conta { get; set; }

    public Saldo()
    {
    }

    public Saldo(decimal valor, Conta conta)
    {
        Valor = valor;
        Conta = conta;
    }
}
