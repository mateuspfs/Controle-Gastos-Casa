using ControleGastosCasa.Application.Mapping;
using ControleGastosCasa.Application.Services;
using ControleGastosCasa.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ControleGastosCasa.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registro do Mapper
        services.AddAutoMapper(typeof(DomainToDtoMapping).Assembly, typeof(DtoToDomainMapping).Assembly);

        // Registro dos services espec�ficos com suas interfaces
        services.AddScoped<IPessoaService, PessoaService>();
        services.AddScoped<ICategoriaService, CategoriaService>();

        return services;
    }
}

