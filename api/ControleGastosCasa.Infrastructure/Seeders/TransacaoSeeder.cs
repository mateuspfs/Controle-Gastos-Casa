using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Domain.Enums;
using ControleGastosCasa.Infrastructure.Persistence;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace ControleGastosCasa.Infrastructure.Seeders;

// Seeder para popular a tabela de transações com dados fictícios
public static class TransacaoSeeder
{
    // Popula o banco com transações geradas aleatoriamente
    public static async Task SeedAsync(AppDbContext context)
    {
        // Verifica se já existem transações no banco
        if (await context.Transacoes.AnyAsync()) return;

        // Busca 5 pessoas do banco
        var pessoas = await context.Pessoas
            .OrderByDescending(p => p.Id)
            .Take(10)
            .ToListAsync();

        if (pessoas.Count == 0) return;

        // Busca todas as categorias
        var categorias = await context.Categorias.ToListAsync();

        if (categorias.Count == 0) return;

        // Separa categorias por tipo
        var categoriasDespesa = categorias
            .Where(c => c.Finalidade == FinalidadeCategoria.Despesa || c.Finalidade == FinalidadeCategoria.Ambas)
            .ToList();

        var categoriasReceita = categorias
            .Where(c => c.Finalidade == FinalidadeCategoria.Receita || c.Finalidade == FinalidadeCategoria.Ambas)
            .ToList();

        var transacoes = new List<Transacao>();
        var faker = new Faker("pt_BR");

        foreach (var pessoa in pessoas)
        {
            var idade = CalcularIdade(pessoa.DataNascimento);
            var podeTerReceita = idade >= 18;

            // Cria 5 transações para cada pessoa
            for (int i = 0; i < 5; i++)
            {
                TipoTransacao tipo;
                Categoria categoria;

                if (podeTerReceita && faker.Random.Bool())
                {
                    // Receita
                    tipo = TipoTransacao.Receita;
                    categoria = faker.PickRandom(categoriasReceita);

                    transacoes.Add(new Transacao
                    {
                        Descricao = GerarDescricaoReceita(faker),
                        Valor = faker.Random.Decimal(500, 5000),
                        Tipo = tipo,
                        DataTransacao = faker.Date.Between(DateTime.Today.AddDays(-60), DateTime.Today),
                        CategoriaId = categoria.Id,
                        PessoaId = pessoa.Id
                    });
                }
                else
                {
                    // Despesa
                    tipo = TipoTransacao.Despesa;
                    categoria = faker.PickRandom(categoriasDespesa);

                    transacoes.Add(new Transacao
                    {
                        Descricao = GerarDescricaoDespesa(faker),
                        Valor = faker.Random.Decimal(10, 500),
                        Tipo = tipo,
                        DataTransacao = faker.Date.Between(DateTime.Today.AddDays(-60), DateTime.Today),
                        CategoriaId = categoria.Id,
                        PessoaId = pessoa.Id
                    });
                }
            }
        }

        // Adiciona no banco
        await context.Transacoes.AddRangeAsync(transacoes);
        await context.SaveChangesAsync();
    }
    private static int CalcularIdade(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - dataNascimento.Year;
        if (dataNascimento.Date > hoje.AddYears(-idade)) idade--;
        return idade;
    }

    private static string GerarDescricaoReceita(Faker faker)
    {
        var descricoes = new[]
        {
            "Salário mensal",
            "Freelance",
            "Rendimento de investimentos",
            "Aluguel recebido",
            "Venda",
            "Comissão de vendas",
            "Auxílio e benefícios",
            "Presente recebido",
            "Reembolso",
            "Prêmio e bonificação"
        };

        return faker.PickRandom(descricoes);
    }

    private static string GerarDescricaoDespesa(Faker faker)
    {
        var descricoes = new[]
        {
            "Compra no {0}",
            "Pagamento de {0}",
            "Conta de {0}",
            "Serviço de {0}",
            "Material de {0}",
            "Refeição no {0}",
            "Transporte - {0}",
            "Medicamento - {0}",
            "Manutenção - {0}",
            "Taxa de {0}"
        };

        var estabelecimentos = new[]
        {
            "supermercado",
            "farmácia",
            "posto de gasolina",
            "restaurante",
            "loja",
            "oficina",
            "clínica",
            "academia",
            "cinema",
            "shopping"
        };

        var descricao = faker.PickRandom(descricoes);
        var estabelecimento = faker.PickRandom(estabelecimentos);
        return string.Format(descricao, estabelecimento);
    }
}

