using System.ComponentModel.DataAnnotations;
using ControleGastosCasa.Domain.Enums;

namespace ControleGastosCasa.Application.Dtos;

public class TransacaoDto
{
    public int? Id { get; set; }

    [Required, StringLength(200, MinimumLength = 2)]
    public string Descricao { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    [Required]
    public TipoTransacao Tipo { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoriaId { get; set; }

    [Range(1, int.MaxValue)]
    public int PessoaId { get; set; }
}

