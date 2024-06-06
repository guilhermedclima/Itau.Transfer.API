using AutoMapper.EquivalencyExpression;
using Itau.Transfer.Application.Config;
using Itau.Transfer.Infrastructure.Context;
using Itau.Transfer.Infrastructure.ErrorHandling;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Sentry.Profiling;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
//Inicialização da camada de application e infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Inicialização do dbContext para o EF
builder.Services.AddDbContext<AppDbContext>(dbContextOptions =>
{
    dbContextOptions.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),
        sqlServerOptions =>
        {
            sqlServerOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });

    dbContextOptions.EnableSensitiveDataLogging(true);
});

//Inicialização do HttpClient para comunicação com a API de Clientes e Contas e Polly para resiliencia
builder.Services.AddHttpClient("ClientesEContasApi", client => { client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiUrl")!); })
    .AddPolicyHandler((provider, request) =>
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();
        return HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, timespan, retryAttempt, context) =>
                {
                    logger.LogWarning(
                        $"Tentando novamente por {timespan.TotalSeconds} segundos, fazendo a tentativa {retryAttempt}.");
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
                        $"Circuit breaker aberto por {timespan.TotalSeconds} segundos devido a {outcome.Exception?.Message}");
                },
                () => { logger.LogInformation("Circuit breaker resetado."); });
    });

//Inicialização do AutoMapper
builder.Services.AddAutoMapper((_, config) =>
{
    config.AddCollectionMappers();
}, AppDomain.CurrentDomain.GetAssemblies());

//Inicialização do Sentry e Sentry Profiling
builder.WebHost.UseSentry(o =>
{
    o.Dsn = builder.Configuration.GetValue<string>("SentryDsn");
    o.Debug = true;
    o.TracesSampleRate = 1.0;
    o.ProfilesSampleRate = 1.0;
    o.AddIntegration(new ProfilingIntegration(
        TimeSpan.FromMilliseconds(500)
    ));
});

//Inicialização do Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console(LogEventLevel.Debug)
    .WriteTo.File("log.txt",
        LogEventLevel.Warning,
        rollingInterval: RollingInterval.Day));

var app = builder.Build();

//Starup do Migrator para rodar as migrations e atualizar ou criar o banco de dados.
DbMigrator.Migrate(app);

//Middleware para tratamento de exceções
app.UseMiddleware(typeof(ExceptionHandlerMiddleware));

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