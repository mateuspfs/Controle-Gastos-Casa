using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Services.Interfaces;
using ControleGastosCasa.Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastosCasa.Api.Controllers;

[ApiController]
[Route("categorias")]
public class CategoriaController(ICategoriaService categoriasService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResult<CategoriaDto>>> CreateAsync([FromBody] CategoriaDto request, CancellationToken cancellationToken)
    {
        var categoria = await categoriasService.CreateAsync(request, cancellationToken);
        if (!categoria.Success || categoria.Data is null)
            return BadRequest(categoria);

        return StatusCode(201, categoria);
    }

    [HttpPut("{id:int}")]
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

    [HttpGet("{id:int}")]
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

    [HttpGet]
    public async Task<ActionResult<ApiResult<PagedResultDto<CategoriaDto>>>> GetAllAsync(
        [FromQuery] int skip = 0, 
        [FromQuery] int take = 20, 
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? finalidade = null,
        CancellationToken cancellationToken = default)
    {
        var categorias = await categoriasService.GetAllAsync(skip, take, searchTerm, finalidade, cancellationToken);
        if (!categorias.Success || categorias.Data is null)
            return BadRequest(categorias);

        return Ok(categorias);
    }
}

