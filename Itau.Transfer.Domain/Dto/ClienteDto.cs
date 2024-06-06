namespace Itau.Transfer.Domain.Dto;

public class ClienteDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string TipoPessoa { get; set; }
}