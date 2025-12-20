using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Infrastructure.Persistence;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace ControleGastosCasa.Infrastructure.Seeders;

// Seeder para popular a tabela de pessoas com dados fictícios
public static class PessoaSeeder
{
    // Popula o banco com pessoas geradas aleatoriamente
    public static async Task SeedAsync(AppDbContext context, int quantidade = 40)
    {
        // Verifica se já existem pessoas no banco
        if (await context.Pessoas.AnyAsync()) return;

        // Configura o Faker para gerar dados brasileiros
        var faker = new Faker<Pessoa>("pt_BR")
            .RuleFor(p => p.Nome, f => f.Person.FullName)
            .RuleFor(p => p.DataNascimento, f => f.Date.Between(
                DateTime.Today.AddYears(-80), // Pessoas entre 0 e 80 anos
                DateTime.Today.AddYears(-1)   // Não nasceu hoje
            ));

        // Gera a quantidade especificada de pessoas
        var pessoas = faker.Generate(quantidade);

        // Adiciona no banco
        await context.Pessoas.AddRangeAsync(pessoas);
        await context.SaveChangesAsync();
    }
}

