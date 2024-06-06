using System.ComponentModel.DataAnnotations;

namespace Itau.Transfer.Domain.Dto;

public class TransferenciaContaDto
{
    public Guid IdOrigem { get; set; }
    public Guid IdDestino { get; set; }
}