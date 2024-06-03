using Itau.Transfer.Application.Config;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("Contas", client => { client.BaseAddress = new Uri("http://localhost:9090/"); })
    .AddPolicyHandler((provider, request) =>
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();
        return HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, timespan, retryAttempt, context) =>
                {
                    logger.LogWarning(
                        $"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}.");
                });
    })
    .AddPolicyHandler((provider, request) =>
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();
        return HttpPolicyExtensions.HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1),
                (outcome, timespan) =>
                {
                    logger.LogWarning(
                        $"Circuit breaker opened for {timespan.TotalSeconds} seconds due to {outcome.Exception?.Message}");
                },
                () => { logger.LogInformation("Circuit breaker reset."); });
    });

builder.Services.AddHttpClient("Transferencias", client => { client.BaseAddress = new Uri("http://localhost:8080/"); })
    .AddPolicyHandler((provider, request) =>
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();
        return HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, timespan, retryAttempt, context) =>
                {
                    logger.LogWarning(
                        $"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}.");
                });
    })
    .AddPolicyHandler((provider, request) =>
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();
        return HttpPolicyExtensions.HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1),
                (outcome, timespan) =>
                {
                    logger.LogWarning(
                        $"Circuit breaker opened for {timespan.TotalSeconds} seconds due to {outcome.Exception?.Message}");
                },
                () => { logger.LogInformation("Circuit breaker reset."); });
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();