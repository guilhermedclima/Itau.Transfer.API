using Itau.Transfer.Application.Interfaces.Services;
using Itau.Transfer.Domain.Contracts;
using Itau.Transfer.Domain.Dto;
using Itau.Transfer.Domain.Entities;
using Itau.Transfer.Domain.Exception;
using Itau.Transfer.Infrastructure.Interfaces;
using Itau.Transfer.Infrastructure.Interfaces.Helpers;
using Itau.Transfer.Infrastructure.Interfaces.Repositories;

namespace Itau.Transfer.Application.Services;

public class TransferenciaService(IHttpClientHelper clientHelper, IClienteService clienteService, IContaService contaService, ITransferenciaRepository transferenciaRepository) : ITransferenciaService
{
    public async Task<TransferenciaResponseDto> TransferenciaAsync(Transferencia request, CancellationToken ct)
    {
        try
        {


            var TransferenciaValidator = new TransferenciaValidator();
            var result = TransferenciaValidator.Validate(request);
            if (!result.IsValid)
            {
                throw new BadRequestException(result.Errors);
            }
            var cliente = await clienteService.GetClienteAsync(request.IdCliente);
            if (cliente == null)
            {
                throw new BadRequestException("Cliente não encontrado");
            }
            var contaOrigem = await contaService.GetContaAsync(request.Conta.IdOrigem);
            if (contaOrigem == null)
            {
                throw new BadRequestException("Conta de origem não encontrada");
            }

            if (!contaOrigem.Ativo)
            {
                throw new BadRequestException("Conta de origem inativa");
            }
            var contadeDestino = await contaService.GetContaAsync(request.Conta.IdDestino);
            if (contadeDestino == null)
            {
                throw new NotFoundException("Conta de destino não encontrada");
            }

            if (!contadeDestino.Ativo)
            {
                throw new BadRequestException("Conta de destino inativa");
            }

            if (contaOrigem.Saldo < request.Valor)
            {
                throw new BadRequestException("Saldo insuficiente");
            }
            if (contaOrigem.LimiteDiario < request.Valor)
            {
                throw new BadRequestException("Limite insuficiente");
            }

            await contaService.AtualizarSaldoAsync(new SaldoDto() { Conta = request.Conta, Valor = request.Valor }, ct);
            request.Id = Guid.NewGuid();
            await transferenciaRepository.InserirTransferenciaAsync(request);
            return new TransferenciaResponseDto() { Id_Transferencia = request.Id };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        //record the transfer on a database and 

        //return guidid

    }
}