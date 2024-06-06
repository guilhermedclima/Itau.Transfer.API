
using System;
using System.Threading;
using System.Threading.Tasks;
using Itau.Transfer.Application.Interfaces.Services;
using Itau.Transfer.Application.Services;
using Itau.Transfer.Domain.Dto;
using Itau.Transfer.Infrastructure.Interfaces.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Itau.Transfer.Tests.UnitTests;

public class ContaServiceTests
{
    private readonly Mock<IHttpClientHelper> _mockHttpClientHelper;
    private readonly Mock<ILogger<ContaService>> _mockLogger;
    private readonly ContaService _contaService;

    public ContaServiceTests()
    {
        _mockHttpClientHelper = new Mock<IHttpClientHelper>();
        _mockLogger = new Mock<ILogger<ContaService>>();
        _contaService = new ContaService(_mockHttpClientHelper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetContaAsync_ValidId_ReturnsContaDto()
    {
        // Arrange
        var contaId = Guid.NewGuid();
        var expectedConta = new ContaDto { Id = contaId };

        _mockHttpClientHelper.Setup(h => h.GetAsync<ContaDto>("ClientesEContasApi", $"contas/{contaId}"))
                             .ReturnsAsync(expectedConta);

        // Act
        var result = await _contaService.GetContaAsync(contaId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedConta.Id, result.Id);

        _mockHttpClientHelper.Verify(h => h.GetAsync<ContaDto>("ClientesEContasApi", $"contas/{contaId}"), Times.Once);
    }

    [Fact]
    public async Task AtualizarSaldoAsync_ValidSaldoDto_ExecutesWithoutException()
    {
        // Arrange
        var saldoDto = new SaldoDto
        {
            Conta = new TransferenciaContaDto()
            {
                IdOrigem = Guid.NewGuid(),
                IdDestino = Guid.NewGuid()
            },
            Valor = 100
        };
        var cancellationToken = CancellationToken.None;

        _mockHttpClientHelper.Setup(h => h.PutAsync("ClientesEContasApi", "contas/saldos", saldoDto, cancellationToken))
                             .Returns(Task.CompletedTask);

        // Act
        await _contaService.AtualizarSaldoAsync(saldoDto, cancellationToken);

        // Assert
        _mockHttpClientHelper.Verify(h => h.PutAsync("ClientesEContasApi", "contas/saldos", saldoDto, cancellationToken), Times.Once);
    }
}
