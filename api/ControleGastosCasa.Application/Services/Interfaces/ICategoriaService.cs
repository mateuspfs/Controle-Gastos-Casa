using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Helpers;

namespace ControleGastosCasa.Application.Services.Interfaces;

public interface ICategoriaService
{
    Task<ApiResult<CategoriaDto>> CreateAsync(CategoriaDto model, CancellationToken cancellationToken = default);
    Task<ApiResult<CategoriaDto>> UpdateAsync(int id, CategoriaDto model, CancellationToken cancellationToken = default);
    Task<ApiResult<PagedResultDto<CategoriaDto>>> GetPaginateAsync(int skip = 0, int take = 20, string? searchTerm = null, int? finalidade = null, CancellationToken cancellationToken = default);
    Task<ApiResult<IReadOnlyList<CategoriaDto>>> GetAllAsync(string? searchTerm = null, int? finalidade = null, CancellationToken cancellationToken = default);
    Task<ApiResult<CategoriaDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}

