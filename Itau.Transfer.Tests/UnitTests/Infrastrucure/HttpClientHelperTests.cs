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
    public async Task GetAsync_ReturnsDeserializedObject_WhenResponseIsSuccess()
    {
        // Arrange
        var clientName = "testClient";
        var path = "http://example.com";
        var responseContent = "{\"key\":\"value\"}";
        var mockHttpMessageHandler = new MockHttpMessageHandler(responseContent, HttpStatusCode.OK);

        var httpClient = new HttpClient(mockHttpMessageHandler);
        _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act
        var result = await _httpClientHelper.GetAsync<Dictionary<string, string>>(clientName, path);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("value", result["key"]);
    }

    [Fact]
    public async Task GetAsync_ThrowsHttpClientRequestException_WhenHttpRequestExceptionOccurs()
    {
        // Arrange
        var clientName = "testClient";
        var path = "http://example.com";
        var mockHttpMessageHandler = new MockHttpMessageHandler(string.Empty, HttpStatusCode.BadRequest);

        var httpClient = new HttpClient(mockHttpMessageHandler);
        _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpClientRequestException>(() =>
            _httpClientHelper.GetAsync<Dictionary<string, string>>(clientName, path));
    }
    [Fact]
    public async Task PostAsync_ThrowsHttpClientRequestException_WhenHttpRequestExceptionOccurs()
    {
        // Arrange
        var clientName = "testClient";
        var path = "http://example.com";
        var body = new { key = "value" };
        var cancellationToken = CancellationToken.None;
        var mockHttpMessageHandler = new MockHttpMessageHandler(string.Empty, HttpStatusCode.BadRequest);

        var httpClient = new HttpClient(mockHttpMessageHandler);
        _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpClientRequestException>(() => _httpClientHelper.PostAsync(clientName, path, body, cancellationToken));
    }
    [Fact]
    public async Task PutAsync_ThrowsHttpClientRequestException_WhenHttpRequestExceptionOccurs()
    {
        // Arrange
        var clientName = "testClient";
        var path = "http://example.com";
        var body = new { key = "value" };
        var cancellationToken = CancellationToken.None;
        var mockHttpMessageHandler = new MockHttpMessageHandler(string.Empty, HttpStatusCode.BadRequest);

        var httpClient = new HttpClient(mockHttpMessageHandler);
        _mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpClientRequestException>(() => _httpClientHelper.PutAsync(clientName, path, body, cancellationToken));
    }


}

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string _responseContent;
    private readonly HttpStatusCode _statusCode;

    public MockHttpMessageHandler(string responseContent, HttpStatusCode statusCode)
    {
        _responseContent = responseContent;
        _statusCode = statusCode;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_responseContent)
        };
        return await Task.FromResult(response);
    }
}