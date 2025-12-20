using System.ComponentModel.DataAnnotations;
using ControleGastosCasa.Domain.Enums;

namespace ControleGastosCasa.Application.Dtos;

public class CategoriaDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Descrição é obrigatória.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Descrição muito curta.")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "Finalidade é obrigatória.")]
    [Range(1, 3, ErrorMessage = "Finalidade deve ser Despesa, Receita ou Ambas.")]
    public FinalidadeCategoria Finalidade { get; set; }
}

