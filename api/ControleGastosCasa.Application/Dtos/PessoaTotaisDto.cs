namespace ControleGastosCasa.Application.Dtos;

public record PessoaTotaisDto(int Id, string Nome, DateTime DataNascimento, int Idade, decimal TotalReceitas, decimal TotalDespesas, decimal Saldo);

public record TotaisPorPessoaResultadoDto(IReadOnlyList<PessoaTotaisDto> Pessoas, decimal TotalReceitas, decimal TotalDespesas, decimal Saldo);

