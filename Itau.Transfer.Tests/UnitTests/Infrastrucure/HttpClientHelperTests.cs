using System.Net;
using System.Text.Json;
using Itau.Transfer.Domain.Exception;
using Itau.Transfer.Infrastructure.HttpClient;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace Itau.Transfer.Tests.UnitTests.Infrastrucure;

public class HttpClientHelperTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<ILogger<HttpClientHelper>> _mockLogger;
    private readonly HttpClientHelper _httpClientHelper;

    public HttpClientHelperTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLogger = new Mock<ILogger<HttpClientHelper>>();
        _httpClientHelper = new HttpClientHelper(_mockHttpClientFactory.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAsync_ValidRequest_ReturnsData()
    {
        // Arrange
        var clientName = "TestClient";
        var path = "http://www.test.com c/path";
        var expectedData = new { Name = "Test" };
        var responseContent = JsonSerializer.Serialize(expectedData);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                  ItExpr.IsNull<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });
        var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        _mockHttpClientFactory.Setup(f => f.CreateClient(clientName))
                              .Returns(httpClient);

        // Act
        var result = await _httpClientHelper.GetAsync<object>(clientName, path);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", ((JsonElement)result).GetProperty("Name").GetString());

        _mockLogger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_NotFound_ReturnsDefault()
    {
        // Arrange
        var clientName = "TestClient";
        var path = "test/path";
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                  ItExpr.IsNull<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        _mockHttpClientFactory.Setup(f => f.CreateClient(clientName))
                              .Returns(httpClient);

        // Act
        var result = await _httpClientHelper.GetAsync<object>(clientName, path);

        // Assert
        Assert.Null(result);

        _mockLogger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_HttpRequestException_ThrowsHttpClientRequestException()
    {
        // Arrange
        var clientName = "TestClient";
        var path = "test/path";
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsNull<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        _mockHttpClientFactory.Setup(f => f.CreateClient(clientName))
                              .Returns(httpClient);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => _httpClientHelper.GetAsync<object>(clientName, path));
        Assert.Equal("Network error", exception.Message);

        _mockLogger.Verify(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task PostAsync_HttpRequestException_ThrowsHttpClientRequestException()
    {
        // Arrange
        var clientName = "TestClient";
        var path = "test/path";
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var body = new { Name = "Test" };

        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                  ItExpr.IsNull<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        _mockHttpClientFactory.Setup(f => f.CreateClient(clientName))
                              .Returns(httpClient);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => _httpClientHelper.PostAsync(clientName, path, body, CancellationToken.None));
        Assert.Equal("Network error", exception.Message);

        _mockLogger.Verify(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task PutAsync_HttpRequestException_ThrowsHttpClientRequestException()
    {
        // Arrange
        var clientName = "TestClient";
        var path = "test/path";
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var body = new { Name = "Test" };

        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                  ItExpr.IsNull<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        _mockHttpClientFactory.Setup(f => f.CreateClient(clientName))
                              .Returns(httpClient);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => _httpClientHelper.PutAsync(clientName, path, body, CancellationToken.None));
        Assert.Equal("Network error", exception.Message);

        _mockLogger.Verify(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }
}
