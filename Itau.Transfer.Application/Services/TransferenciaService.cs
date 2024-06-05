using AutoMapper;
using Itau.Transfer.Application.Interfaces.Services;
using Itau.Transfer.Domain.Contracts;
using Itau.Transfer.Domain.Dto;
using Itau.Transfer.Domain.Entities;
using Itau.Transfer.Domain.Exception;
using Itau.Transfer.Infrastructure.Interfaces.Repositories;

namespace Itau.Transfer.Application.Services;

public class TransferenciaService(IClienteService clienteService,
        IContaService contaService, ITransferenciaRepository transferenciaRepository, IMapper mapper)
    : ITransferenciaService
{
    public async Task<TransferenciaResponseDto> TransferenciaAsync(TransferenciaDto request, CancellationToken ct)
    {
        try
        {
            await ValidateRequestAsync(request);

            var cliente = await clienteService.GetClienteAsync(request.IdCliente);
            if (cliente == null)
            {
                throw new BadRequestException("Cliente não encontrado");
            }

            var contaOrigem = await contaService.GetContaAsync(request.Conta.IdOrigem);
            ContaOrigemValidators(contaOrigem, request.Valor);

            var contadeDestino = await contaService.GetContaAsync(request.Conta.IdDestino);
            ContaDestinoValidators(contadeDestino);

            await UpdateSaldoAsync(request, ct);
            await InsertTransferenciaAsync(request);

            return new TransferenciaResponseDto { Id_Transferencia = request.Id };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void ContaDestinoValidators(ContaDto contadeDestino)
    {
        if (contadeDestino == null)
        {
            throw new NotFoundException("Conta de destino não encontrada");
        }

        if (!contadeDestino.Ativo)
        {
            throw new BadRequestException("Conta de destino inativa");
        }
    }

    private static void ContaOrigemValidators(ContaDto contaOrigem, decimal valor)
    {
        if (contaOrigem == null)
        {
            throw new BadRequestException("Conta de origem não encontrada");
        }

        if (!contaOrigem.Ativo)
        {
            throw new BadRequestException("Conta de origem inativa");
        }

        if (contaOrigem.Saldo < valor)
        {
            throw new BadRequestException("Saldo insuficiente");
        }

        if (contaOrigem.LimiteDiario < valor)
        {
            throw new BadRequestException("Limite insuficiente");
        }
    }

    private async Task ValidateRequestAsync(TransferenciaDto request)
    {
        var validator = new TransferenciaDtoValidator();
        var result = validator.Validate(request);
        if (!result.IsValid)
        {
            throw new BadRequestException(result.Errors);
        }
    }

    private async Task UpdateSaldoAsync(TransferenciaDto request, CancellationToken ct)
    {
        await contaService.AtualizarSaldoAsync(new SaldoDto { Conta = request.Conta, Valor = request.Valor }, ct);
    }

    private async Task InsertTransferenciaAsync(TransferenciaDto request)
    {
        request.Id = Guid.NewGuid();
        await transferenciaRepository.InserirTransferenciaAsync(mapper.Map<Transferencia>(request));
    }
}