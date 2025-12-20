using ControleGastosCasa.Infrastructure.Persistence;
using ControleGastosCasa.Infrastructure.Repositories;
using ControleGastosCasa.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ControleGastosCasa.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Registro genérico do repositório base
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // Registro dos repositórios específicos com suas interfaces
        services.AddScoped<IPessoaRepository, PessoaRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<ITransacaoRepository, TransacaoRepository>();

        return services;
    }
}

