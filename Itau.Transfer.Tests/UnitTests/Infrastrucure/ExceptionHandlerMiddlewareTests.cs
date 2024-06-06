using FluentValidation.Results;
using Itau.Transfer.Domain.Exception;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Net;
using Itau.Transfer.Infrastructure.ErrorHandling;
using Xunit;

namespace Itau.Transfer.Tests.UnitTests.Infrastrucure;

 public class ExceptionHandlerMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly Mock<ILogger<ExceptionHandlerMiddleware>> _loggerMock;
        private readonly DefaultHttpContext _httpContext;
        private readonly ExceptionHandlerMiddleware _middleware;

        public ExceptionHandlerMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<ExceptionHandlerMiddleware>>();
            _httpContext = new DefaultHttpContext();
            _middleware = new ExceptionHandlerMiddleware(_nextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Invoke_CallsNextMiddleware()
        {
            // Arrange
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await _middleware.Invoke(_httpContext);

            // Assert
            _nextMock.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async Task Invoke_HandlesNotFoundException()
        {
            // Arrange
            var exception = new NotFoundException("Not Found");
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Throws(exception);

            // Act
            await _middleware.Invoke(_httpContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, _httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_HandlesBadRequestException_WithValidationErrors()
        {
            // Arrange
            var errors = new List<ValidationFailure> { new ValidationFailure("Property", "Error") };
            var exception = new Itau.Transfer.Domain.Exception.BadRequestException(errors);
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Throws(exception);
        
            // Act
            await _middleware.Invoke(_httpContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
            Assert.Equal("application/json", _httpContext.Response.ContentType);
        }

        [Fact]
        public async Task Invoke_HandlesHttpClientRequestException()
        {
            // Arrange
            var exception = new HttpClientRequestException("Gateway Timeout");
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Throws(exception);

            // Act
            await _middleware.Invoke(_httpContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.GatewayTimeout, _httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_HandlesGenericException()
        {
            // Arrange
            var exception = new Exception("Internal Server Error");
            _nextMock.Setup(next => next(It.IsAny<HttpContext>())).Throws(exception);

            // Act
            await _middleware.Invoke(_httpContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
            
        }

       
        private async Task<string> GetResponseBody()
        {
            _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_httpContext.Response.Body);
            return await reader.ReadToEndAsync();
        }
    }