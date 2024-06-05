using System.ComponentModel.DataAnnotations;

namespace Itau.Transfer.Domain.Dto;

public class TransferenciaContaDto
{
    [Key]
    public Guid Id { get; set; }

    public Guid IdOrigem { get; set; }
    public Guid IdDestino { get; set; }
}