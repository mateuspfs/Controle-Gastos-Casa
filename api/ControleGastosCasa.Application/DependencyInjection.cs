using ControleGastosCasa.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace ControleGastosCasa.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DomainToDtoMapping).Assembly, typeof(DtoToDomainMapping).Assembly);
        return services;
    }
}

