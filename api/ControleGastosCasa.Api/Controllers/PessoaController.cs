using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Services.Interfaces;
using ControleGastosCasa.Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastosCasa.Api.Controllers;

[ApiController]
[Route("api/pessoas")]
public class PessoaController(IPessoaService pessoasService) : ControllerBase
{
    /// <summary>
    /// Cria uma nova pessoa
    /// </summary>
    /// <param name="request">Dados da pessoa a ser criada</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Pessoa criada com sucesso</returns>
    /// <response code="201">Pessoa criada com sucesso</response>
    /// <response code="400">Erro de validação ou dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<PessoaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult<PessoaDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResult<PessoaDto>>> CreateAsync([FromBody] PessoaDto request, CancellationToken cancellationToken)
    {        
        var pessoa = await pessoasService.CreateAsync(request, cancellationToken);
        if (!pessoa.Success || pessoa.Data is null)
            return BadRequest(pessoa);

        return StatusCode(201, pessoa);
    }

    /// <summary>
    /// Atualiza uma pessoa existente
    /// </summary>
    /// <param name="id">ID da pessoa a ser atualizada</param>
    /// <param name="request">Dados atualizados da pessoa</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Pessoa atualizada com sucesso</returns>
    /// <response code="200">Pessoa atualizada com sucesso</response>
    /// <response code="400">Erro de validação ou dados inválidos</response>
    /// <response code="404">Pessoa não encontrada</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<PessoaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<PessoaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<PessoaDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<PessoaDto>>> UpdateAsync(int id, [FromBody] PessoaDto request, CancellationToken cancellationToken)
    {
        var pessoa = await pessoasService.UpdateAsync(id, request, cancellationToken);
        if (!pessoa.Success || pessoa.Data is null)
        {
            if (pessoa.Errors.Any(e => e.Contains("não encontrada", StringComparison.OrdinalIgnoreCase)))
                return NotFound(pessoa);
            return BadRequest(pessoa);
        }

        return Ok(pessoa);
    }

    /// <summary>
    /// Obtém uma pessoa pelo ID
    /// </summary>
    /// <param name="id">ID da pessoa</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados da pessoa</returns>
    /// <response code="200">Pessoa encontrada</response>
    /// <response code="400">Erro na requisição</response>
    /// <response code="404">Pessoa não encontrada</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<PessoaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<PessoaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<PessoaDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<PessoaDto>>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var pessoa = await pessoasService.GetByIdAsync(id, cancellationToken);
        if (!pessoa.Success || pessoa.Data is null)
        {
            if (pessoa.Errors.Any(e => e.Contains("não encontrada", StringComparison.OrdinalIgnoreCase)))
                return NotFound(pessoa);
            return BadRequest(pessoa);
        }

        return Ok(ApiResult<PessoaDto>.Ok(pessoa.Data));
    }

    /// <summary>
    /// Lista todas as pessoas com paginação e busca opcional
    /// </summary>
    /// <param name="skip">Número de registros a pular (padrão: 0)</param>
    /// <param name="take">Número de registros a retornar (padrão: 20). Use 0 para retornar todos</param>
    /// <param name="searchTerm">Termo de busca para filtrar por nome (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de pessoas</returns>
    /// <response code="200">Lista de pessoas retornada com sucesso</response>
    /// <response code="400">Erro na requisição</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PagedResultDto<PessoaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<PagedResultDto<PessoaDto>>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetAllAsync(
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 20, 
        [FromQuery] string? searchTerm = null, 
        CancellationToken cancellationToken = default)
    {
        // Se take for 0, retorna todos os registros (sem paginação)
        if (take == 0)
        {
            var pessoas = await pessoasService.GetAllAsync(searchTerm, cancellationToken);
            if (!pessoas.Success || pessoas.Data is null)
                return BadRequest(pessoas);

            return Ok(pessoas);
        }

        // Caso contrário, retorna paginado
        var pessoasPaginadas = await pessoasService.GetPaginateAsync(skip, take, searchTerm, cancellationToken);
        if (!pessoasPaginadas.Success || pessoasPaginadas.Data is null)
            return BadRequest(pessoasPaginadas);

        return Ok(pessoasPaginadas);
    }

    /// <summary>
    /// Remove uma pessoa pelo ID
    /// </summary>
    /// <param name="id">ID da pessoa a ser removida</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de exclusão</returns>
    /// <response code="200">Pessoa removida com sucesso</response>
    /// <response code="400">Erro na requisição</response>
    /// <response code="404">Pessoa não encontrada</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deletou = await pessoasService.DeleteAsync(id, cancellationToken);
        if (!deletou.Success)
            return BadRequest(deletou);

        if (deletou.Data == false)
            return NotFound();

        return Ok(ApiResult<bool>.Ok(true));
    }
}

