﻿using FluentValidation;
using Itau.Transfer.Domain.Dto;

namespace Itau.Transfer.Domain.Contracts;

public class TransferenciaDtoValidator : AbstractValidator<TransferenciaDto>
{
    public TransferenciaDtoValidator()
    {
        RuleFor(x => x.IdCliente).NotEmpty().WithMessage("IdCliente é obrigatório");
        RuleFor(x => x.Valor).NotEmpty().WithMessage("Valor é obrigatório").GreaterThan(0).WithMessage("Valor deve ser maior que zero");
        RuleFor(x => x.Conta.IdDestino).NotNull().WithMessage("Conta de destino é obrigatória");
        RuleFor(x => x.Conta.IdOrigem).NotNull().WithMessage("Conta de origem é obrigatória");
    }
}