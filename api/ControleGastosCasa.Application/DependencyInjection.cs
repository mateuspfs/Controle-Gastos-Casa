using ControleGastosCasa.Application.Mapping;
using ControleGastosCasa.Application.Services;
using ControleGastosCasa.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ControleGastosCasa.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DomainToDtoMapping).Assembly, typeof(DtoToDomainMapping).Assembly);
        services.AddScoped<IPessoaService, PessoaService>();
        return services;
    }
}

