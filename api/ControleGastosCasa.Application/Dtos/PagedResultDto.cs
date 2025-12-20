namespace ControleGastosCasa.Application.Dtos;

// DTO de resposta paginada com todas as informações necessárias para o frontend
public class PagedResultDto<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

