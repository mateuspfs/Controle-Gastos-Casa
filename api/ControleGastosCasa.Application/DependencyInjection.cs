using ControleGastosCasa.Application.Services.Interfaces;
using ControleGastosCasa.Application.Services;
using ControleGastosCasa.Application.Services.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace ControleGastosCasa.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMapper, SimpleMapper>();
        return services;
    }
}

