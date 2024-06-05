using FluentValidation.Results;
using Itau.Transfer.Domain.Exception;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sentry;
using System.Net;

namespace Itau.Transfer.Infrastructure.ErrorHandling;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    protected virtual Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case NotFoundException _:
                return CreateTextResponse(context, HttpStatusCode.NotFound, exception.Message);

            case BadRequestException ex:
                SentrySdk.CaptureException(exception);
                if (ex.Errors.Any())
                    return CreateValidationFailureResponse(context, HttpStatusCode.BadRequest, ex.Errors);

                return CreateTextResponse(context, HttpStatusCode.BadRequest, exception.Message);

            case HttpClientRequestException _:
                SentrySdk.CaptureException(exception);
                return CreateTextResponse(context, HttpStatusCode.GatewayTimeout, exception.Message);

            default:
                _logger.LogError(exception, "ExceptionMessage: {exceptionMessage}", exception.Message);
                SentrySdk.CaptureException(exception);

                return CreateTextResponse(context, HttpStatusCode.InternalServerError, exception.Message);
        }
    }

    private static Task CreateTextResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "text/plain";
        return context.Response.WriteAsync(message);
    }

    private static Task CreateValidationFailureResponse(HttpContext context, HttpStatusCode statusCode, IReadOnlyCollection<ValidationFailure> errors)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        return context.Response.WriteAsync(JsonConvert.SerializeObject(errors));
    }
}