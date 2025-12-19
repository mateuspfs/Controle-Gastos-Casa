using AutoMapper;
using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Helpers;
using ControleGastosCasa.Application.Services.Interfaces;
using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Domain.Repositories;

namespace ControleGastosCasa.Application.Services;

public class PessoaService(IGenericRepository<Pessoa> pessoasRepository, IMapper mapper) : IPessoaService
{
    // Cria uma nova pessoa.
    public async Task<ApiResult<PessoaDto>> CreateAsync(PessoaDto model, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = mapper.Map<PessoaDto, Pessoa>(model);

            await pessoasRepository.AddAsync(pessoa, cancellationToken);
            return ApiResult<PessoaDto>.Ok(mapper.Map<Pessoa, PessoaDto>(pessoa));
        }
        catch
        {
            return ApiResult<PessoaDto>.Fail("Ocorreu um erro ao criar a pessoa");
        }
    }

    // Retorna pessoas paginadas (skip/take).
    public async Task<ApiResult<IReadOnlyList<PessoaDto>>> GetAllAsync(int skip = 0, int take = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoas = await pessoasRepository.PaginateAsync(skip, take, cancellationToken);
            var mapped = pessoas.Select(p => mapper.Map<Pessoa, PessoaDto>(p)).ToList();
            return ApiResult<IReadOnlyList<PessoaDto>>.Ok(mapped);
        }
        catch
        {
            return ApiResult<IReadOnlyList<PessoaDto>>.Fail("Ocorreu um erro ao listar pessoas");
        }
    }

    // Remove uma pessoa, se existir.
    public async Task<ApiResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await pessoasRepository.GetByIdAsync(id, cancellationToken);
            if (pessoa is null)
                return ApiResult<bool>.Fail("Pessoa não encontrada");

            await pessoasRepository.DeleteAsync(pessoa, cancellationToken);
            return ApiResult<bool>.Ok(true);
        }
        catch
        {
            return ApiResult<bool>.Fail("Ocorreu um erro ao excluir a pessoa");
        }
    }

    // Busca uma pessoa por id.
    public async Task<ApiResult<PessoaDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await pessoasRepository.GetByIdAsync(id, cancellationToken);
            if (pessoa is null)
                return ApiResult<PessoaDto>.Fail("Pessoa não encontrada");

            return ApiResult<PessoaDto>.Ok(mapper.Map<Pessoa, PessoaDto>(pessoa));
        }
        catch
        {
            return ApiResult<PessoaDto>.Fail("Ocorreu um erro ao buscar a pessoa");
        }
    }
}

