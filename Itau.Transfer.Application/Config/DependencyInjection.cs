using Itau.Transfer.Application.Interfaces.Services;
using Itau.Transfer.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Itau.Transfer.Application.Config;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IContaService, ContaService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<ITransferenciaService, TransferenciaService>();
        return services;
    }
}