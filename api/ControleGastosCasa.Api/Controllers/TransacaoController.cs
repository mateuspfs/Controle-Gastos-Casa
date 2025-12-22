using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Services.Interfaces;
using ControleGastosCasa.Application.Helpers;
using Microsoft.AspNetCore.Mvc;
using ControleGastosCasa.Domain.Enums;

namespace ControleGastosCasa.Api.Controllers;

[ApiController]
[Route("api/transacoes")]
public class TransacaoController(ITransacaoService transacoesService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResult<TransacaoDto>>> CreateAsync([FromBody] TransacaoDto request, CancellationToken cancellationToken)
    {
        var transacao = await transacoesService.CreateAsync(request, cancellationToken);

        if (!transacao.Success || transacao.Data is null)
            return BadRequest(transacao);

        return StatusCode(201, transacao);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResult<TransacaoDto>>> UpdateAsync(int id, [FromBody] TransacaoDto request, CancellationToken cancellationToken)
    {
        var transacao = await transacoesService.UpdateAsync(id, request, cancellationToken);
        if (!transacao.Success || transacao.Data is null)
        {
            if (transacao.Errors.Any(e => e.Contains("não encontrada", StringComparison.OrdinalIgnoreCase)))
                return NotFound(transacao);
            return BadRequest(transacao);
        }

        return Ok(transacao);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResult<TransacaoDto>>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var transacao = await transacoesService.GetByIdAsync(id, cancellationToken);
        if (!transacao.Success || transacao.Data is null)
        {
            if (transacao.Errors.Any(e => e.Contains("não encontrada", StringComparison.OrdinalIgnoreCase)))
                return NotFound(transacao);
            return BadRequest(transacao);
        }

        return Ok(ApiResult<TransacaoDto>.Ok(transacao.Data));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<PagedResultDto<TransacaoDto>>>> GetAllAsync(
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 20, 
        [FromQuery] DateTime? dataInicio = null, 
        [FromQuery] DateTime? dataFim = null,
        [FromQuery] int? pessoaId = null,
        [FromQuery] int? categoriaId = null,
        [FromQuery] TipoTransacao? tipo = null,
        CancellationToken cancellationToken = default)
    {
        var transacoesPaginadas = await transacoesService.GetPaginateAsync(skip, take, dataInicio, dataFim, pessoaId, categoriaId, tipo, cancellationToken);
        if (!transacoesPaginadas.Success || transacoesPaginadas.Data is null)
            return BadRequest(transacoesPaginadas);

        return Ok(transacoesPaginadas);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deletou = await transacoesService.DeleteAsync(id, cancellationToken);
        if (!deletou.Success)
            return BadRequest(deletou);

        if (!deletou.Data)
            return NotFound();

        return Ok(ApiResult<bool>.Ok(true));
    }

    [HttpGet("totais-gerais")]
    public async Task<ActionResult<ApiResult<TotaisGeraisDto>>> GetTotaisGeraisAsync(
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null,
        [FromQuery] int? pessoaId = null,
        [FromQuery] int? categoriaId = null,
        [FromQuery] TipoTransacao? tipo = null,
        CancellationToken cancellationToken = default)
    {
        var totais = await transacoesService.GetTotaisGeraisAsync(dataInicio, dataFim, pessoaId, categoriaId, tipo, cancellationToken);
        if (!totais.Success || totais.Data is null)
            return BadRequest(totais);

        return Ok(totais);
    }
}

