
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Itau.Transfer.Application.Interfaces.Services;
using Itau.Transfer.Application.Services;
using Itau.Transfer.Domain.Dto;
using Itau.Transfer.Domain.Entities;
using Itau.Transfer.Domain.Exception;
using Itau.Transfer.Infrastructure.Interfaces.Repositories;
using Moq;
using Xunit;

namespace Itau.Transfer.Tests.UnitTests;
public class TransferenciaServiceTests
{
    private readonly Mock<IClienteService> _mockClienteService;
    private readonly Mock<IContaService> _mockContaService;
    private readonly Mock<ITransferenciaRepository> _mockTransferenciaRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ITransferenciaService _transferenciaService;

    public TransferenciaServiceTests()
    {
        _mockClienteService = new Mock<IClienteService>();
        _mockContaService = new Mock<IContaService>();
        _mockTransferenciaRepository = new Mock<ITransferenciaRepository>();
        _mockMapper = new Mock<IMapper>();
        _transferenciaService = new TransferenciaService(
            _mockClienteService.Object,
            _mockContaService.Object,
            _mockTransferenciaRepository.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task TransferenciaAsync_ShouldReturnTransferenciaResponse_WhenRequestIsValid()
    {
        // Arrange
        var request = new TransferenciaDto
        {
            IdCliente = Guid.NewGuid(),
            Conta = new TransferenciaContaDto()
            {
                IdOrigem = Guid.NewGuid(),
                IdDestino = Guid.NewGuid()
            },
            Valor = 100
        };

        var cliente = new ClienteDto { Id = request.IdCliente };
        var contaOrigem = new ContaDto { Id = request.Conta.IdOrigem, Saldo = 200, LimiteDiario = 500, Ativo = true };
        var contaDestino = new ContaDto { Id = request.Conta.IdDestino, Ativo = true };

        _mockClienteService.Setup(s => s.GetClienteAsync(request.IdCliente)).ReturnsAsync(cliente);
        _mockContaService.Setup(s => s.GetContaAsync(request.Conta.IdOrigem)).ReturnsAsync(contaOrigem);
        _mockContaService.Setup(s => s.GetContaAsync(request.Conta.IdDestino)).ReturnsAsync(contaDestino);
        _mockMapper.Setup(m => m.Map<Transferencia>(It.IsAny<TransferenciaDto>())).Returns(new Transferencia());

        // Act
        var result = await _transferenciaService.TransferenciaAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Id, result.Id_Transferencia);

        _mockClienteService.Verify(s => s.GetClienteAsync(request.IdCliente), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdOrigem), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdDestino), Times.Once);
        _mockTransferenciaRepository.Verify(r => r.InserirTransferenciaAsync(It.IsAny<Transferencia>()), Times.Once);
    }
    [Fact]
    public async Task TransferenciaAsync_ShouldThrowBadRequestException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new TransferenciaDto
        {
            
            Conta = new TransferenciaContaDto()
            {
                IdOrigem = Guid.NewGuid(),
                IdDestino = Guid.NewGuid()
            },
            Valor = 100
        };

        var validator = new Mock<IValidator<TransferenciaDto>>();
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Property", "Error message")
        });

        validator.Setup(v => v.ValidateAsync(It.IsAny<TransferenciaDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var transferenciaService = new TransferenciaService(
            _mockClienteService.Object,
            _mockContaService.Object,
            _mockTransferenciaRepository.Object,
            _mockMapper.Object);
       

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => transferenciaService.TransferenciaAsync(request, CancellationToken.None));

        _mockClienteService.Verify(s => s.GetClienteAsync(It.IsAny<Guid>()), Times.Never);
        _mockContaService.Verify(s => s.GetContaAsync(It.IsAny<Guid>()), Times.Never);
        _mockTransferenciaRepository.Verify(r => r.InserirTransferenciaAsync(It.IsAny<Transferencia>()), Times.Never);
    }
    [Fact]
    public async Task TransferenciaAsync_ShouldThrowBadRequestException_WhenClienteNotFound()
    {
        // Arrange
        var request = new TransferenciaDto
        {
            IdCliente = Guid.NewGuid(),
            Conta = new TransferenciaContaDto
            {
                IdOrigem = Guid.NewGuid(),
                IdDestino = Guid.NewGuid()
            },
            Valor = 100
        };

        _mockClienteService.Setup(s => s.GetClienteAsync(request.IdCliente)).ReturnsAsync((ClienteDto)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _transferenciaService.TransferenciaAsync(request, CancellationToken.None));

        _mockClienteService.Verify(s => s.GetClienteAsync(request.IdCliente), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(It.IsAny<Guid>()), Times.Never);
        _mockTransferenciaRepository.Verify(r => r.InserirTransferenciaAsync(It.IsAny<Transferencia>()), Times.Never);
    }

    [Fact]
    public async Task TransferenciaAsync_ShouldThrowBadRequestException_WhenContaOrigemNotFound()
    {
        // Arrange
        var request = new TransferenciaDto
        {
            IdCliente = Guid.NewGuid(),
            Conta = new TransferenciaContaDto
            {
                IdOrigem = Guid.NewGuid(),
                IdDestino = Guid.NewGuid()
            },
            Valor = 100
        };

        var cliente = new ClienteDto { Id = request.IdCliente };

        _mockClienteService.Setup(s => s.GetClienteAsync(request.IdCliente)).ReturnsAsync(cliente);
        _mockContaService.Setup(s => s.GetContaAsync(request.Conta.IdOrigem)).ReturnsAsync((ContaDto)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _transferenciaService.TransferenciaAsync(request, CancellationToken.None));

        _mockClienteService.Verify(s => s.GetClienteAsync(request.IdCliente), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdOrigem), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdDestino), Times.Never);
        _mockTransferenciaRepository.Verify(r => r.InserirTransferenciaAsync(It.IsAny<Transferencia>()), Times.Never);
    }

    [Fact]
    public async Task TransferenciaAsync_ShouldThrowBadRequestException_WhenContaOrigemInativa()
    {
        // Arrange
        var request = new TransferenciaDto
        {
            IdCliente = Guid.NewGuid(),
            Conta = new TransferenciaContaDto
            {
                IdOrigem = Guid.NewGuid(),
                IdDestino = Guid.NewGuid()
            },
            Valor = 100
        };

        var cliente = new ClienteDto { Id = request.IdCliente };
        var contaOrigem = new ContaDto { Id = request.Conta.IdOrigem, Ativo = false };

        _mockClienteService.Setup(s => s.GetClienteAsync(request.IdCliente)).ReturnsAsync(cliente);
        _mockContaService.Setup(s => s.GetContaAsync(request.Conta.IdOrigem)).ReturnsAsync(contaOrigem);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _transferenciaService.TransferenciaAsync(request, CancellationToken.None));

        _mockClienteService.Verify(s => s.GetClienteAsync(request.IdCliente), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdOrigem), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdDestino), Times.Never);
        _mockTransferenciaRepository.Verify(r => r.InserirTransferenciaAsync(It.IsAny<Transferencia>()), Times.Never);
    }

    [Fact]
    public async Task TransferenciaAsync_ShouldThrowBadRequestException_WhenSaldoInsuficiente()
    {
        // Arrange
        var request = new TransferenciaDto
        {
            IdCliente = Guid.NewGuid(),
            Conta = new TransferenciaContaDto
            {
                IdOrigem = Guid.NewGuid(),
                IdDestino = Guid.NewGuid()
            },
            Valor = 300
        };

        var cliente = new ClienteDto { Id = request.IdCliente };
        var contaOrigem = new ContaDto { Id = request.Conta.IdOrigem, Saldo = 200, Ativo = true };

        _mockClienteService.Setup(s => s.GetClienteAsync(request.IdCliente)).ReturnsAsync(cliente);
        _mockContaService.Setup(s => s.GetContaAsync(request.Conta.IdOrigem)).ReturnsAsync(contaOrigem);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _transferenciaService.TransferenciaAsync(request, CancellationToken.None));

        _mockClienteService.Verify(s => s.GetClienteAsync(request.IdCliente), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdOrigem), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdDestino), Times.Never);
        _mockTransferenciaRepository.Verify(r => r.InserirTransferenciaAsync(It.IsAny<Transferencia>()), Times.Never);
    }

    [Fact]
    public async Task TransferenciaAsync_ShouldThrowBadRequestException_WhenLimiteDiarioInsuficiente()
    {
        // Arrange
        var request = new TransferenciaDto
        {
            IdCliente = Guid.NewGuid(),
            Conta = new TransferenciaContaDto
            {
                IdOrigem = Guid.NewGuid(),
                IdDestino = Guid.NewGuid()
            },
            Valor = 600
        };

        var cliente = new ClienteDto { Id = request.IdCliente };
        var contaOrigem = new ContaDto { Id = request.Conta.IdOrigem, Saldo = 1000, LimiteDiario = 500, Ativo = true };

        _mockClienteService.Setup(s => s.GetClienteAsync(request.IdCliente)).ReturnsAsync(cliente);
        _mockContaService.Setup(s => s.GetContaAsync(request.Conta.IdOrigem)).ReturnsAsync(contaOrigem);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _transferenciaService.TransferenciaAsync(request, CancellationToken.None));

        _mockClienteService.Verify(s => s.GetClienteAsync(request.IdCliente), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdOrigem), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdDestino), Times.Never);
        _mockTransferenciaRepository.Verify(r => r.InserirTransferenciaAsync(It.IsAny<Transferencia>()), Times.Never);
    }

    [Fact]
    public async Task TransferenciaAsync_ShouldThrowBadRequestException_WhenContaDestinoNotFound()
    {
        // Arrange
        var request = new TransferenciaDto
        {
            IdCliente = Guid.NewGuid(),
            Conta = new TransferenciaContaDto
            {
                IdOrigem = Guid.NewGuid(),
                IdDestino = Guid.NewGuid()
            },
            Valor = 100
        };

        var cliente = new ClienteDto { Id = request.IdCliente };
        var contaOrigem = new ContaDto { Id = request.Conta.IdOrigem, Saldo = 200, LimiteDiario = 500, Ativo = true };

        _mockClienteService.Setup(s => s.GetClienteAsync(request.IdCliente)).ReturnsAsync(cliente);
        _mockContaService.Setup(s => s.GetContaAsync(request.Conta.IdOrigem)).ReturnsAsync(contaOrigem);
        _mockContaService.Setup(s => s.GetContaAsync(request.Conta.IdDestino)).ReturnsAsync((ContaDto)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _transferenciaService.TransferenciaAsync(request, CancellationToken.None));

        _mockClienteService.Verify(s => s.GetClienteAsync(request.IdCliente), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdOrigem), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdDestino), Times.Once);
        _mockTransferenciaRepository.Verify(r => r.InserirTransferenciaAsync(It.IsAny<Transferencia>()), Times.Never);
    }

    [Fact]
    public async Task TransferenciaAsync_ShouldThrowBadRequestException_WhenContaDestinoInativa()
    {
        // Arrange
        var request = new TransferenciaDto
        {
            IdCliente = Guid.NewGuid(),
            Conta = new TransferenciaContaDto
            {
                IdOrigem = Guid.NewGuid(),
                IdDestino = Guid.NewGuid()
            },
            Valor = 100
        };

        var cliente = new ClienteDto { Id = request.IdCliente };
        var contaOrigem = new ContaDto { Id = request.Conta.IdOrigem, Saldo = 200, LimiteDiario = 500, Ativo = true };
        var contaDestino = new ContaDto { Id = request.Conta.IdDestino, Ativo = false };

        _mockClienteService.Setup(s => s.GetClienteAsync(request.IdCliente)).ReturnsAsync(cliente);
        _mockContaService.Setup(s => s.GetContaAsync(request.Conta.IdOrigem)).ReturnsAsync(contaOrigem);
        _mockContaService.Setup(s => s.GetContaAsync(request.Conta.IdDestino)).ReturnsAsync(contaDestino);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _transferenciaService.TransferenciaAsync(request, CancellationToken.None));

        _mockClienteService.Verify(s => s.GetClienteAsync(request.IdCliente), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdOrigem), Times.Once);
        _mockContaService.Verify(s => s.GetContaAsync(request.Conta.IdDestino), Times.Once);
        _mockTransferenciaRepository.Verify(r => r.InserirTransferenciaAsync(It.IsAny<Transferencia>()), Times.Never);
    }
}
