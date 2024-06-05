using Itau.Transfer.Infrastructure.HttpClient;
using Itau.Transfer.Infrastructure.Interfaces.Helpers;
using Itau.Transfer.Infrastructure.Interfaces.Repositories;
using Itau.Transfer.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Itau.Transfer.Application.Config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IHttpClientHelper, HttpClientHelper>();
            services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
            services.AddScoped<IRepositoryBase, RepositoryBase>();

            return services;
        }
    }
}