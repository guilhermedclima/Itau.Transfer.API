using Itau.Transfer.API.Controllers;
using Itau.Transfer.Application.Interfaces.Services;
using Itau.Transfer.Domain.Dto;
using Itau.Transfer.Domain.Exception;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Itau.Transfer.Tests.IntergratedTests
{
    public class TransferenciaControllerTests
    {
        private readonly TransferenciaController _controller;

        public TransferenciaControllerTests(ITransferenciaService service)
        {
            _controller = new TransferenciaController(service);
        }

        [Fact]
        public async Task RealizaTransferencia_ReturnsOkResult()
        {
            // Arrange
             var transferenciaDto = new TransferenciaDto
            {
                IdCliente = Guid.Parse("2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f"),
                Valor = 500,
                Conta = new TransferenciaContaDto
                {
                    IdOrigem = Guid.Parse("d0d32142-74b7-4aca-9c68-838aeacef96b"),
                    IdDestino = Guid.Parse("41313d7b-bd75-4c75-9dea-1f4be434007f")
                }
            };

            // Act
            var result = await _controller.RealizaTransferencia(transferenciaDto, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

       [Fact]
        public async Task RealizaTransferencia_InvalidRequest_ReturnsBadRequest_Valor_Acima_Limite()
        {
            // Arrange
            var transferenciaDto = new TransferenciaDto
            {
                IdCliente = Guid.Parse("2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f"),
                Valor = 1000,
                Conta = new TransferenciaContaDto
                {
                    IdOrigem = Guid.Parse("d0d32142-74b7-4aca-9c68-838aeacef96b"),
                    IdDestino = Guid.Parse("41313d7b-bd75-4c75-9dea-1f4be434007f")
                }
            };
            // Act
            var exception =  await  Assert.ThrowsAsync<BadRequestException>(() => _controller.RealizaTransferencia(transferenciaDto, CancellationToken.None));
            
            // Assert
            Assert.Equal("Limite insuficiente", exception.Message);
        }
        [Fact]
        public async Task RealizaTransferencia_InvalidRequest_ReturnsBadRequest_Valor_Acima_Saldo()
        {
            // Arrange
            var transferenciaDto = new TransferenciaDto
            {
                IdCliente = Guid.Parse("2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f"),
                Valor = 1000,
                Conta = new TransferenciaContaDto
                {
                    IdOrigem = Guid.Parse("41313d7b-bd75-4c75-9dea-1f4be434007f"),
                    IdDestino = Guid.Parse("41313d7b-bd75-4c75-9dea-1f4be434007f")
                }
            };
            // Act
            var exception =  await  Assert.ThrowsAsync<BadRequestException>(() => _controller.RealizaTransferencia(transferenciaDto, CancellationToken.None));
            
            // Assert
            Assert.Equal("Saldo insuficiente", exception.Message);
        }
        [Fact]
        public async Task RealizaTransferencia_InvalidRequest_ReturnsBadRequest_Client_Nao_Existe()
        {
            // Arrange
            var transferenciaDto = new TransferenciaDto
            {
                IdCliente = Guid.Parse("2ceb26e9-7b5c-417e-bf75-ffaa66e3a77f"),
                Valor = 500,
                Conta = new TransferenciaContaDto
                {
                    IdOrigem = Guid.Parse("d0d32142-74b7-4aca-9c68-838aeacef96b"),
                    IdDestino = Guid.Parse("41313d7b-bd75-4c75-9dea-1f4be434007f")
                }
            };
            // Act
            var exception =  await Assert.ThrowsAsync<BadRequestException>(() => _controller.RealizaTransferencia(transferenciaDto, CancellationToken.None));

            // Assert
            Assert.Equal("Cliente não encontrado", exception.Message);
        }
        [Fact]
        public async Task RealizaTransferencia_InvalidRequest_ReturnsBadRequest_Conta_Origem_Nao_Existe()
        {
            // Arrange
            var transferenciaDto = new TransferenciaDto
            {
                IdCliente = Guid.Parse("2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f"),
                Valor = 500,
                Conta = new TransferenciaContaDto
                {
                    IdOrigem = Guid.Parse("d0d32142-74b7-4aca-9c68-838aeacef9ab"),
                    IdDestino = Guid.Parse("41313d7b-bd75-4c75-9dea-1f4be434007f")
                }
            };
            // Act
            var exception =  await Assert.ThrowsAsync<BadRequestException>(() => _controller.RealizaTransferencia(transferenciaDto, CancellationToken.None));

            // Assert
            Assert.Equal("Conta de origem não encontrada", exception.Message);
        }
        [Fact]
        public async Task RealizaTransferencia_InvalidRequest_ReturnsBadRequest_Conta_Deswtino_Nao_Existe()
        {
            // Arrange
            var transferenciaDto = new TransferenciaDto
            {
                IdCliente = Guid.Parse("2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f"),
                Valor = 500,
                Conta = new TransferenciaContaDto
                {
                    IdOrigem = Guid.Parse("d0d32142-74b7-4aca-9c68-838aeacef96b"),
                    IdDestino = Guid.Parse("41313d7b-bd75-4c75-9dea-1f4be434008f")
                }
            };
            // Act
            var exception =  await Assert.ThrowsAsync<BadRequestException>(() => _controller.RealizaTransferencia(transferenciaDto, CancellationToken.None));

            // Assert
            Assert.Equal("Conta de destino não encontrada", exception.Message);
        }
    }
}