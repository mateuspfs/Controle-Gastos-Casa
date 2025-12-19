using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Helpers;

namespace ControleGastosCasa.Application.Services.Interfaces;

public interface IPessoaService
{
    Task<ApiResult<PessoaDto>> CreateAsync(PessoaDto model, CancellationToken cancellationToken = default);
    Task<ApiResult<IReadOnlyList<PessoaDto>>> GetAllAsync(int skip = 0, int take = 20, CancellationToken cancellationToken = default);
    Task<ApiResult<PessoaDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

