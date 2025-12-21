using System.ComponentModel.DataAnnotations;
using ControleGastosCasa.Domain.Enums;

namespace ControleGastosCasa.Application.Dtos;

public class TransacaoDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Descrição muito curta ou muito longa.")]
    public string Descricao { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Tipo de transação é obrigatório.")]
    public TipoTransacao Tipo { get; set; }

    [Required(ErrorMessage = "Data da transação é obrigatória.")]
    public DateTime DataTransacao { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Categoria é obrigatória.")]
    public int CategoriaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Pessoa é obrigatória.")]
    public int PessoaId { get; set; }
    
    // Propriedades de navegação
    public PessoaDto? Pessoa { get; set; }
    public CategoriaDto? Categoria { get; set; }
}

