using ControleGastosCasa.Domain.Enums;

namespace ControleGastosCasa.Domain.Entities;

public class Categoria
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public FinalidadeCategoria Finalidade { get; set; }
}

