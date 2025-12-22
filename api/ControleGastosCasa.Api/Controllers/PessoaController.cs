using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Services.Interfaces;
using ControleGastosCasa.Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastosCasa.Api.Controllers;

[ApiController]
[Route("pessoas")]
public class PessoaController(IPessoaService pessoasService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ApiResult<PessoaDto>>> CreateAsync([FromBody] PessoaDto request, CancellationToken cancellationToken)
    {        
        var pessoa = await pessoasService.CreateAsync(request, cancellationToken);
        if (!pessoa.Success || pessoa.Data is null)
            return BadRequest(pessoa);

        return StatusCode(201, pessoa);
    }

    [HttpPut("{id:int}")]
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

    [HttpGet("{id:int}")]
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

    [HttpGet]
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deletou = await pessoasService.DeleteAsync(id, cancellationToken);
        if (!deletou.Success)
            return BadRequest(deletou);

        if (deletou.Data == false)
            return NotFound();

        return Ok(ApiResult<bool>.Ok(true));
    }

    [HttpGet("totais-gerais")]
    public async Task<ActionResult<ApiResult<TotaisGeraisDto>>> GetTotaisGeraisAsync(CancellationToken cancellationToken)
    {
        var totais = await pessoasService.GetTotaisGeraisAsync(cancellationToken);
        if (!totais.Success || totais.Data is null)
            return BadRequest(totais);

        return Ok(totais);
    }
}

