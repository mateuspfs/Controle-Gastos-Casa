using ControleGastosCasa.Domain.Enums;

namespace ControleGastosCasa.Application.Dtos;

public record CategoriaTotaisDto(int Id, string Descricao, FinalidadeCategoria Finalidade, decimal TotalReceitas, decimal TotalDespesas, decimal Saldo);

