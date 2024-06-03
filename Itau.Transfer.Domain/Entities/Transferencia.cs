namespace Itau.Transfer.Domain.Entities;
public class Transferencia
{
    public Guid IdCliente { get; set; }
    public decimal Valor { get; set; }
    public Conta Conta { get; set; }

    public Transferencia()
    {
    }

    public Transferencia(Guid idCliente, decimal valor, Conta conta)
    {
        IdCliente = idCliente;
        Valor = valor;
        Conta = conta;
    }
}
