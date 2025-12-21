using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Helpers;
using ControleGastosCasa.Domain.Enums;

namespace ControleGastosCasa.Application.Services.Interfaces;

public interface ITransacaoService
{
    Task<ApiResult<TransacaoDto>> CreateAsync(
        TransacaoDto model,
        CancellationToken cancellationToken = default);

    Task<ApiResult<TransacaoDto>> UpdateAsync(
        int id,
        TransacaoDto model,
        CancellationToken cancellationToken = default);

    Task<ApiResult<PagedResultDto<TransacaoDto>>> GetPaginateAsync(
        int skip = 0, 
        int take = 20, 
        DateTime? dataInicio = null, 
        DateTime? dataFim = null,
        int? pessoaId = null,
        int? categoriaId = null,
        TipoTransacao? tipo = null,
        CancellationToken cancellationToken = default);

    Task<ApiResult<TransacaoDto>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ApiResult<bool>> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}

