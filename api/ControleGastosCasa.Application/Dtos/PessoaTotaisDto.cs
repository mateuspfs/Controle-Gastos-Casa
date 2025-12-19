namespace ControleGastosCasa.Application.Dtos;

public record PessoaTotaisDto(int PessoaId, string Nome, decimal TotalReceitas, decimal TotalDespesas, decimal Saldo);

public record TotaisPorPessoaResultadoDto(IReadOnlyList<PessoaTotaisDto> Pessoas, decimal TotalReceitas, decimal TotalDespesas, decimal Saldo);

