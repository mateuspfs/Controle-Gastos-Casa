using System.ComponentModel.DataAnnotations;
using ControleGastosCasa.Domain.Enums;

namespace ControleGastosCasa.Application.Dtos;

public class CategoriaDto
{
    public int? Id { get; set; }

    [Required, StringLength(150, MinimumLength = 2)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public FinalidadeCategoria Finalidade { get; set; }
}

