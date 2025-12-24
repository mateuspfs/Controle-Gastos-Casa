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
    /// <summary>
    /// Cria uma nova transação financeira
    /// </summary>
    /// <param name="request">Dados da transação a ser criada</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Transação criada com sucesso</returns>
    /// <response code="201">Transação criada com sucesso</response>
    /// <response code="400">Erro de validação ou dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<TransacaoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult<TransacaoDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResult<TransacaoDto>>> CreateAsync([FromBody] TransacaoDto request, CancellationToken cancellationToken)
    {
        var transacao = await transacoesService.CreateAsync(request, cancellationToken);

        if (!transacao.Success || transacao.Data is null)
            return BadRequest(transacao);

        return StatusCode(201, transacao);
    }

    /// <summary>
    /// Atualiza uma transação existente
    /// </summary>
    /// <param name="id">ID da transação a ser atualizada</param>
    /// <param name="request">Dados atualizados da transação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Transação atualizada com sucesso</returns>
    /// <response code="200">Transação atualizada com sucesso</response>
    /// <response code="400">Erro de validação ou dados inválidos</response>
    /// <response code="404">Transação não encontrada</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<TransacaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<TransacaoDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<TransacaoDto>), StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Obtém uma transação pelo ID
    /// </summary>
    /// <param name="id">ID da transação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados da transação</returns>
    /// <response code="200">Transação encontrada</response>
    /// <response code="400">Erro na requisição</response>
    /// <response code="404">Transação não encontrada</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<TransacaoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<TransacaoDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<TransacaoDto>), StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Lista todas as transações com paginação e filtros opcionais
    /// </summary>
    /// <param name="skip">Número de registros a pular (padrão: 0)</param>
    /// <param name="take">Número de registros a retornar (padrão: 20)</param>
    /// <param name="dataInicio">Data inicial para filtrar transações (opcional)</param>
    /// <param name="dataFim">Data final para filtrar transações (opcional)</param>
    /// <param name="pessoaId">ID da pessoa para filtrar transações (opcional)</param>
    /// <param name="categoriaId">ID da categoria para filtrar transações (opcional)</param>
    /// <param name="tipo">Tipo de transação: 1 = Despesa, 2 = Receita (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de transações</returns>
    /// <response code="200">Lista de transações retornada com sucesso</response>
    /// <response code="400">Erro na requisição</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PagedResultDto<TransacaoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<PagedResultDto<TransacaoDto>>), StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Remove uma transação pelo ID
    /// </summary>
    /// <param name="id">ID da transação a ser removida</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de exclusão</returns>
    /// <response code="200">Transação removida com sucesso</response>
    /// <response code="400">Erro na requisição</response>
    /// <response code="404">Transação não encontrada</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deletou = await transacoesService.DeleteAsync(id, cancellationToken);
        if (!deletou.Success)
            return BadRequest(deletou);

        if (!deletou.Data)
            return NotFound();

        return Ok(ApiResult<bool>.Ok(true));
    }

    /// <summary>
    /// Obtém os totais gerais de receitas, despesas e saldo líquido com filtros opcionais
    /// </summary>
    /// <param name="dataInicio">Data inicial para calcular totais (opcional)</param>
    /// <param name="dataFim">Data final para calcular totais (opcional)</param>
    /// <param name="pessoaId">ID da pessoa para filtrar totais (opcional)</param>
    /// <param name="categoriaId">ID da categoria para filtrar totais (opcional)</param>
    /// <param name="tipo">Tipo de transação: 1=Despesa, 2=Receita (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Totais gerais (receitas, despesas e saldo líquido)</returns>
    /// <response code="200">Totais calculados com sucesso</response>
    /// <response code="400">Erro na requisição</response>
    [HttpGet("totais-gerais")]
    [ProducesResponseType(typeof(ApiResult<TotaisGeraisDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<TotaisGeraisDto>), StatusCodes.Status400BadRequest)]
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

