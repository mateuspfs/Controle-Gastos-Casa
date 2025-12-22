using System;
using System.Linq.Expressions;
using AutoMapper;
using ControleGastosCasa.Application.Dtos;
using ControleGastosCasa.Application.Helpers;
using ControleGastosCasa.Application.Services.Interfaces;
using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Domain.Enums;
using ControleGastosCasa.Infrastructure.Repositories.Interfaces;

namespace ControleGastosCasa.Application.Services;

public class PessoaService(
    IPessoaRepository pessoasRepository,
    ITransacaoRepository transacoesRepository,
    IMapper mapper) : IPessoaService
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

    // Atualiza uma pessoa existente.
    public async Task<ApiResult<PessoaDto>> UpdateAsync(int id, PessoaDto model, CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoaExistente = await pessoasRepository.GetByIdAsync(id, cancellationToken);
            if (pessoaExistente is null)
                return ApiResult<PessoaDto>.Fail("Pessoa não encontrada");

            // Atualiza os campos
            pessoaExistente.Nome = model.Nome;
            pessoaExistente.DataNascimento = model.DataNascimento;

            await pessoasRepository.UpdateAsync(pessoaExistente, cancellationToken);
            return ApiResult<PessoaDto>.Ok(mapper.Map<Pessoa, PessoaDto>(pessoaExistente));
        }
        catch
        {
            return ApiResult<PessoaDto>.Fail("Ocorreu um erro ao atualizar a pessoa");
        }
    }

    // Retorna pessoas paginadas com filtro opcional por nome
    public async Task<ApiResult<PagedResultDto<PessoaTotaisDto>>> GetPaginateAsync(int skip = 0, int take = 20, string? searchTerm = null, CancellationToken cancellationToken = default)
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

            // Ordena por Id
            Expression<Func<Pessoa, object>> orderBy = p => p.Id;

            // Busca os itens paginados e o total de registros sequencialmente
            var pessoas = await pessoasRepository.PaginateAsync(safeSkip, safeTake, where, orderBy, OrderDirection.Descending, cancellationToken);
            var totalItems = await pessoasRepository.CountAsync(where, cancellationToken);

            List<PessoaTotaisDto> mapped = [];
            foreach (var pessoa in pessoas)
            {                
                // Calcula totais usando os métodos do repository
                var receitas = await transacoesRepository.SomarTransacoesPorTipoAsync(TipoTransacao.Receita, pessoa.Id, cancellationToken);
                var despesas = await transacoesRepository.SomarTransacoesPorTipoAsync(TipoTransacao.Despesa, pessoa.Id, cancellationToken);
                
                // Cria o DTO completo com os totais calculados
                mapped.Add(new PessoaTotaisDto(
                    pessoa.Id,
                    pessoa.Nome,
                    pessoa.DataNascimento,
                    DateHelper.CalcularIdade(pessoa.DataNascimento),
                    receitas,
                    despesas,
                    receitas - despesas
                ));
            }

            // Calcula informações de paginação
            var currentPage = (safeSkip / safeTake) + 1;
            var totalPages = (int)Math.Ceiling(totalItems / (double)safeTake);
            var hasPreviousPage = currentPage > 1;
            var hasNextPage = currentPage < totalPages;

            var result = new PagedResultDto<PessoaTotaisDto>
            {
                Items = mapped,
                CurrentPage = currentPage,
                PageSize = safeTake,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = hasPreviousPage,
                HasNextPage = hasNextPage,
            };

            return ApiResult<PagedResultDto<PessoaTotaisDto>>.Ok(result);
        }
        catch(Exception e)
        {
            return ApiResult<PagedResultDto<PessoaTotaisDto>>.Fail("Ocorreu um erro ao listar pessoas");
        }
    }

    // Retorna todas as pessoas com filtro opcional por nome (sem paginação)
    public async Task<ApiResult<IReadOnlyList<PessoaDto>>> GetAllAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Normaliza o termo de busca (remove espaços extras)
            var normalizedSearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();

            // Cria expressão de filtro where se searchTerm fornecido
            Expression<Func<Pessoa, bool>>? where = null;
            if (!string.IsNullOrWhiteSpace(normalizedSearchTerm)) where = p => p.Nome.Contains(normalizedSearchTerm);
            // Ordena por nome ascendente
            Expression<Func<Pessoa, object>> orderBy = p => p.Nome;

            // Busca todos os registros usando GetAllAsync do repository
            var pessoas = await pessoasRepository.GetAllAsync(where, orderBy, OrderDirection.Ascending, cancellationToken);

            // Mapeia os DTOs
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

    // Retorna os totais gerais de todas as pessoas
    public async Task<ApiResult<TotaisGeraisDto>> GetTotaisGeraisAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var totalReceitas = await transacoesRepository.SomarTransacoesPorTipoAsync(TipoTransacao.Receita, null, cancellationToken);
            var totalDespesas = await transacoesRepository.SomarTransacoesPorTipoAsync(TipoTransacao.Despesa, null, cancellationToken);
            var saldoLiquido = totalReceitas - totalDespesas;

            var totais = new TotaisGeraisDto
            {
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                SaldoLiquido = saldoLiquido
            };

            return ApiResult<TotaisGeraisDto>.Ok(totais);
        }
        catch
        {
            return ApiResult<TotaisGeraisDto>.Fail("Ocorreu um erro ao obter totais gerais");
        }
    }
}

