 
using Autofac.Extensions.DependencyInjection;
using AutoMapper.EquivalencyExpression;
using AutoMapper;
using Itau.Transfer.Application.Config;
using Itau.Transfer.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace Itau.Transfer.Tests
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }
        public static void ConfigureHost(IHostBuilder hostBuilder)
        {
            var builder = hostBuilder.ConfigureAppConfiguration(lb => lb.AddJsonFile("appsettings.json", false, true))
                .UseServiceProviderFactory(new AutofacServiceProviderFactory());
        }
        public void ConfigureServices(IServiceCollection services)
        {
            //Inicialização da camada de application e infrastructure
            services.AddApplication();
            services.AddInfrastructure();

            services.AddAutoMapper((serviceProvider, config) =>
            {
                config.AddCollectionMappers();
                config.UseEntityFrameworkCoreModel<AppDbContext>(serviceProvider);
            }, AppDomain.CurrentDomain.GetAssemblies());
            
            services.AddHttpClient("ClientesEContasApi", client => { client.BaseAddress = new Uri(Configuration.GetValue<string>("ApiUrl")!); })
                .AddPolicyHandler((provider, request) =>
                {
                    var logger = provider.GetRequiredService<ILogger<Startup>>();
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
                    var logger = provider.GetRequiredService<ILogger<Startup>>();
                    return HttpPolicyExtensions.HandleTransientHttpError()
                        .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1),
                            (outcome, timespan) =>
                            {
                                logger.LogWarning(
                                    $"Circuit breaker aberto por {timespan.TotalSeconds} segundos devido a {outcome.Exception?.Message}");
                            },
                            () => { logger.LogInformation("Circuit breaker resetado."); });
                });
            //services.AddApplication(Configuration);

            //services.AddInfrastructure(Configuration);
       //     services.Configure<StorageAccountConfig>(Configuration.GetSection("StorageAccount"));
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName: "FTSDev"));
        }

        //public static void Configure(IServiceProvider provider, ITestOutputHelperAccessor accessor)
        //{
        //    var scope = provider.CreateScope();
        //    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

        //    DataGenerator.Initialize(context);
        //}
    }
}
