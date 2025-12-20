using System.ComponentModel.DataAnnotations;

namespace ControleGastosCasa.Application.Dtos;

public class PessoaDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Nome muito curto.")]
    [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "Nome não deve conter números.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data de nascimento é obrigatória.")]
    public DateTime DataNascimento { get; set; }

    // Idade formatada como texto (ex: "5 anos", "6 meses", "2 anos e 3 meses")
    public string? Idade { get; set; }
}

