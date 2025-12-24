using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Services.Interfaces;
using ControleGastosCasa.Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastosCasa.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriaController(ICategoriaService categoriasService) : ControllerBase
{
    /// <summary>
    /// Cria uma nova categoria
    /// </summary>
    /// <param name="request">Dados da categoria a ser criada</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Categoria criada com sucesso</returns>
    /// <response code="201">Categoria criada com sucesso</response>
    /// <response code="400">Erro de validação ou dados inválidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResult<CategoriaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult<CategoriaDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResult<CategoriaDto>>> CreateAsync([FromBody] CategoriaDto request, CancellationToken cancellationToken)
    {
        var categoria = await categoriasService.CreateAsync(request, cancellationToken);
        if (!categoria.Success || categoria.Data is null)
            return BadRequest(categoria);

        return StatusCode(201, categoria);
    }

    /// <summary>
    /// Atualiza uma categoria existente
    /// </summary>
    /// <param name="id">ID da categoria a ser atualizada</param>
    /// <param name="request">Dados atualizados da categoria</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Categoria atualizada com sucesso</returns>
    /// <response code="200">Categoria atualizada com sucesso</response>
    /// <response code="400">Erro de validação ou dados inválidos</response>
    /// <response code="404">Categoria não encontrada</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<CategoriaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<CategoriaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<CategoriaDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<CategoriaDto>>> UpdateAsync(int id, [FromBody] CategoriaDto request, CancellationToken cancellationToken)
    {
        var categoria = await categoriasService.UpdateAsync(id, request, cancellationToken);
        if (!categoria.Success || categoria.Data is null)
        {
            if (categoria.Errors.Any(e => e.Contains("não encontrada", StringComparison.OrdinalIgnoreCase)))
                return NotFound(categoria);
            return BadRequest(categoria);
        }

        return Ok(categoria);
    }

    /// <summary>
    /// Obtém uma categoria pelo ID
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados da categoria</returns>
    /// <response code="200">Categoria encontrada</response>
    /// <response code="400">Erro na requisição</response>
    /// <response code="404">Categoria não encontrada</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResult<CategoriaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<CategoriaDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<CategoriaDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<CategoriaDto>>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var categoria = await categoriasService.GetByIdAsync(id, cancellationToken);
        if (!categoria.Success || categoria.Data is null)
        {
            if (categoria.Errors.Any(e => e.Contains("não encontrada", StringComparison.OrdinalIgnoreCase)))
                return NotFound(categoria);
                
            return BadRequest(categoria);
        }

        return Ok(ApiResult<CategoriaDto>.Ok(categoria.Data));
    }

    /// <summary>
    /// Lista todas as categorias com paginação, busca e filtro por finalidade opcionais
    /// </summary>
    /// <param name="skip">Número de registros a pular (padrão: 0)</param>
    /// <param name="take">Número de registros a retornar (padrão: 20). Use 0 para retornar todos</param>
    /// <param name="searchTerm">Termo de busca para filtrar por descrição (opcional)</param>
    /// <param name="finalidade">Filtro por finalidade: 1=Despesa, 2=Receita, 3=Ambas (opcional)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de categorias</returns>
    /// <response code="200">Lista de categorias retornada com sucesso</response>
    /// <response code="400">Erro na requisição</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<PagedResultDto<CategoriaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<PagedResultDto<CategoriaDto>>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetAllAsync(
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 20, 
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? finalidade = null,
        CancellationToken cancellationToken = default)
    {
        // Se take for 0, retorna todos os registros (sem paginação)
        if (take == 0)
        {
            var categorias = await categoriasService.GetAllAsync(searchTerm, finalidade, cancellationToken);
            if (!categorias.Success || categorias.Data is null)
                return BadRequest(categorias);

            return Ok(categorias);
        }

        // Caso contrário, retorna paginado
        var categoriasPaginadas = await categoriasService.GetPaginateAsync(skip, take, searchTerm, finalidade, cancellationToken);
        if (!categoriasPaginadas.Success || categoriasPaginadas.Data is null)
            return BadRequest(categoriasPaginadas);

        return Ok(categoriasPaginadas);
    }
}

