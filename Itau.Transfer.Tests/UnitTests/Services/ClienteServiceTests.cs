using Itau.Transfer.Application.Services;
using Itau.Transfer.Domain.Dto;
using Itau.Transfer.Infrastructure.Interfaces.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
namespace Itau.Transfer.Tests.UnitTests;

public class ClienteServiceTests
{
    private readonly Mock<IHttpClientHelper> _mockHttpClientHelper;
    private readonly Mock<ILogger<ClienteService>> _mockLogger;
    private readonly ClienteService _clienteService;

    public ClienteServiceTests()
    {
        _mockHttpClientHelper = new Mock<IHttpClientHelper>();
        _mockLogger = new Mock<ILogger<ClienteService>>();
        _clienteService = new ClienteService(_mockHttpClientHelper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetClienteAsync_ValidId_ReturnsClienteDto()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var expectedCliente = new ClienteDto
            { Id = clienteId, Nome = "Cliente Teste", TipoPessoa = "Fisica", Telefone = "11999999999" };

        _mockHttpClientHelper.Setup(h => h.GetAsync<ClienteDto>("ClientesEContasApi", $"clientes/{clienteId}"))
                             .ReturnsAsync(expectedCliente);

        // Act
        var result = await _clienteService.GetClienteAsync(clienteId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCliente.Id, result.Id);
        Assert.Equal(expectedCliente.Nome, result.Nome);
        Assert.Equal(expectedCliente.TipoPessoa, result.TipoPessoa);
        Assert.Equal(expectedCliente.Telefone, result.Telefone);

        _mockHttpClientHelper.Verify(h => h.GetAsync<ClienteDto>("ClientesEContasApi", $"clientes/{clienteId}"), Times.Once);
    }

    [Fact]
    public async Task GetClienteAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var clienteId = Guid.NewGuid();

        _mockHttpClientHelper.Setup(h => h.GetAsync<ClienteDto>("ClientesEContasApi", $"clientes/{clienteId}"))
                             .ReturnsAsync((ClienteDto)null);

        // Act
        var result = await _clienteService.GetClienteAsync(clienteId);

        // Assert
        Assert.Null(result);

        _mockHttpClientHelper.Verify(h => h.GetAsync<ClienteDto>("ClientesEContasApi", $"clientes/{clienteId}"), Times.Once);
    }
}
