using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Domain.Repositories;
using ControleGastosCasa.Infrastructure.Persistence;
using ControleGastosCasa.Infrastructure.Repositories;
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

        services.AddScoped<IGenericRepository<Pessoa>, PessoaRepository>();
        services.AddScoped<IGenericRepository<Categoria>, CategoriaRepository>();
        services.AddScoped<IGenericRepository<Transacao>, TransacaoRepository>();

        return services;
    }
}

