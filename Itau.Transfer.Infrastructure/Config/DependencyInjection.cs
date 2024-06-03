using Itau.Transfer.Application.Interfaces;
using Itau.Transfer.Infrastructure.HttpClient;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Itau.Transfer.Application.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IHttpClientHelper, HttpClientHelper>();

            
            return services;
        }
    }
}
