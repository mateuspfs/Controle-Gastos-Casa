using System;
using System.Linq.Expressions;
using AutoMapper;
using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Helpers;
using ControleGastosCasa.Application.Services.Interfaces;
using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Infrastructure.Repositories.Interfaces;

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

    // Retorna pessoas paginadas com filtro opcional por nome
    public async Task<ApiResult<PagedResultDto<PessoaDto>>> GetAllAsync(int skip = 0, int take = 20, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Garante valores seguros para paginação
            var safeSkip = Math.Max(0, skip);
            var safeTake = Math.Max(1, take);

            // Normaliza o termo de busca (remove espaços extras)
            var normalizedSearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();

            // Cria expressão de filtro where se searchTerm fornecido
            Expression<Func<Pessoa, bool>>? where = null;
            if (!string.IsNullOrWhiteSpace(normalizedSearchTerm)) where = p => p.Nome.Contains(normalizedSearchTerm);

            // Busca os itens paginados e o total de registros sequencialmente
            var pessoas = await pessoasRepository.PaginateAsync(safeSkip, safeTake, where, cancellationToken);
            var totalItems = await pessoasRepository.CountAsync(where, cancellationToken);

            // Mapeia os DTOs
            var mapped = pessoas.Select(p => mapper.Map<Pessoa, PessoaDto>(p)).ToList();

            // Calcula informações de paginação
            var currentPage = (safeSkip / safeTake) + 1;
            var totalPages = (int)Math.Ceiling(totalItems / (double)safeTake);
            var hasPreviousPage = currentPage > 1;
            var hasNextPage = currentPage < totalPages;

            var result = new PagedResultDto<PessoaDto>
            {
                Items = mapped,
                CurrentPage = currentPage,
                PageSize = safeTake,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = hasPreviousPage,
                HasNextPage = hasNextPage,
            };

            return ApiResult<PagedResultDto<PessoaDto>>.Ok(result);
        }
        catch(Exception e)
        {
            return ApiResult<PagedResultDto<PessoaDto>>.Fail("Ocorreu um erro ao listar pessoas");
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

