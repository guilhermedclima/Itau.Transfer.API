namespace Itau.Transfer.Domain.Dto;

public class ClienteDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string TipoPessoa { get; set; }

    public ClienteDto()
    {
    }

    public ClienteDto(Guid id, string nome, string telefone, string tipoPessoa)
    {
        Id = id;
        Nome = nome;
        Telefone = telefone;
        TipoPessoa = tipoPessoa;
    }
}