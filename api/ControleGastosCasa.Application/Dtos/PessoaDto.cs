using System.ComponentModel.DataAnnotations;

namespace ControleGastosCasa.Application.Dtos;

public class PessoaDto
{
    public int? Id { get; set; }

    [Required, StringLength(150, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Idade { get; set; }
}

