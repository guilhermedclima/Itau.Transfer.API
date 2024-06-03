namespace Itau.Transfer.Domain.Entities;
public class Cliente
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string TipoPessoa { get; set; }

    public Cliente()
    {
    }

    public Cliente(Guid id, string nome, string telefone, string tipoPessoa)
    {
        Id = id;
        Nome = nome;
        Telefone = telefone;
        TipoPessoa = tipoPessoa;
    }
}
