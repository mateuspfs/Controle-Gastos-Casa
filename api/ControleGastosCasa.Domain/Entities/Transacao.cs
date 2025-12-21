using ControleGastosCasa.Domain.Enums;

namespace ControleGastosCasa.Domain.Entities;

public class Transacao
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public TipoTransacao Tipo { get; set; }
    public DateTime DataTransacao { get; set; }
    public int CategoriaId { get; set; }
    public int PessoaId { get; set; }
    
    // Propriedades de navegação
    public Pessoa Pessoa { get; set; } = null!;
    public Categoria Categoria { get; set; } = null!;
}

