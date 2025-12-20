using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Domain.Enums;
using ControleGastosCasa.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ControleGastosCasa.Infrastructure.Seeders;

// Seeder para popular a tabela de categorias com exemplos reais
public static class CategoriaSeeder
{
    // Popula o banco com categorias de exemplo
    public static async Task SeedAsync(AppDbContext context)
    {
        // Verifica se já existem categorias no banco
        if (await context.Categorias.AnyAsync()) return;

        // Lista de categorias com exemplos reais
        var categorias = new List<Categoria>
        {
            // Categorias de Despesa
            new() { Descricao = "Alimentação", Finalidade = FinalidadeCategoria.Despesa },
            new() { Descricao = "Transporte", Finalidade = FinalidadeCategoria.Despesa },
            new() { Descricao = "Moradia", Finalidade = FinalidadeCategoria.Despesa },
            new() { Descricao = "Educação", Finalidade = FinalidadeCategoria.Despesa },
            new() { Descricao = "Saúde", Finalidade = FinalidadeCategoria.Despesa },
            new() { Descricao = "Lazer", Finalidade = FinalidadeCategoria.Despesa },
            new() { Descricao = "Utilidades", Finalidade = FinalidadeCategoria.Despesa },
            new() { Descricao = "Vestuário", Finalidade = FinalidadeCategoria.Despesa },
            new() { Descricao = "Impostos", Finalidade = FinalidadeCategoria.Despesa },
            new() { Descricao = "Combustível", Finalidade = FinalidadeCategoria.Despesa },
            
            // Categorias de Receita
            new() { Descricao = "Salário", Finalidade = FinalidadeCategoria.Receita },
            new() { Descricao = "Freelance", Finalidade = FinalidadeCategoria.Receita },
            new() { Descricao = "Investimentos", Finalidade = FinalidadeCategoria.Receita },
            new() { Descricao = "Aluguel Recebido", Finalidade = FinalidadeCategoria.Receita },
            new() { Descricao = "Vendas", Finalidade = FinalidadeCategoria.Receita },
            new() { Descricao = "Benefícios", Finalidade = FinalidadeCategoria.Receita },
            new() { Descricao = "Comissões", Finalidade = FinalidadeCategoria.Receita },
            new() { Descricao = "Presentes Recebidos", Finalidade = FinalidadeCategoria.Receita },
            
            // Categorias que podem ser Ambas
            new() { Descricao = "Transferências", Finalidade = FinalidadeCategoria.Ambas },
            new() { Descricao = "Outros", Finalidade = FinalidadeCategoria.Ambas },
        };

        // Adiciona no banco
        await context.Categorias.AddRangeAsync(categorias);
        await context.SaveChangesAsync();
    }
}

