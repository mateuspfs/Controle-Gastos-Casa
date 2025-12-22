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

public class TransacaoService(
    ITransacaoRepository transacoesRepository,
    IPessoaRepository pessoasRepository,
    ICategoriaRepository categoriasRepository,
    IMapper mapper) : ITransacaoService
{
    // Cria uma transacao aplicando regras de negócio.
    public async Task<ApiResult<TransacaoDto>> CreateAsync(
        TransacaoDto model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pessoa = await pessoasRepository.GetByIdAsync(model.PessoaId, cancellationToken);
            if (pessoa is null)
                return ApiResult<TransacaoDto>.Fail("Pessoa não encontrada");

            var categoria = await categoriasRepository.GetByIdAsync(model.CategoriaId, cancellationToken);
            if (categoria is null)
                return ApiResult<TransacaoDto>.Fail("Categoria não encontrada");

            // Calcula idade a partir da data de nascimento
            var idade = Helpers.DateHelper.CalcularIdade(pessoa.DataNascimento);
            
            // Menor de idade nao pode registrar receita.
            if (idade < 18 && model.Tipo == TipoTransacao.Receita)
                return ApiResult<TransacaoDto>.Fail("Menor de idade só pode registrar despesa");

            // Finalidade da categoria deve corresponder ao tipo.
            var erroFinalidade = ValidarFinalidade(model.Tipo, categoria.Finalidade);
            if (erroFinalidade is not null)
                return ApiResult<TransacaoDto>.Fail(erroFinalidade);

            var transacao = mapper.Map<TransacaoDto, Transacao>(model);

            await transacoesRepository.AddAsync(transacao, cancellationToken);
            return ApiResult<TransacaoDto>.Ok(mapper.Map<Transacao, TransacaoDto>(transacao));
        }
        catch
        {
            return ApiResult<TransacaoDto>.Fail("Ocorreu um erro ao criar a transação");
        }
    }

    // Atualiza uma transação existente.
    public async Task<ApiResult<TransacaoDto>> UpdateAsync(int id, TransacaoDto model, CancellationToken cancellationToken = default)
    {
        try
        {
            var transacaoExistente = await transacoesRepository.GetByIdAsync(id, cancellationToken);
            if (transacaoExistente is null)
                return ApiResult<TransacaoDto>.Fail("Transação não encontrada");

            var pessoa = await pessoasRepository.GetByIdAsync(model.PessoaId, cancellationToken);
            if (pessoa is null)
                return ApiResult<TransacaoDto>.Fail("Pessoa não encontrada");

            var categoria = await categoriasRepository.GetByIdAsync(model.CategoriaId, cancellationToken);
            if (categoria is null)
                return ApiResult<TransacaoDto>.Fail("Categoria não encontrada");

            // Calcula idade a partir da data de nascimento
            var idade = Helpers.DateHelper.CalcularIdade(pessoa.DataNascimento);
            
            // Menor de idade nao pode registrar receita.
            if (idade < 18 && model.Tipo == TipoTransacao.Receita)
                return ApiResult<TransacaoDto>.Fail("Menor de idade só pode registrar despesa");

            // Finalidade da categoria deve corresponder ao tipo.
            var erroFinalidade = ValidarFinalidade(model.Tipo, categoria.Finalidade);
            if (erroFinalidade is not null)
                return ApiResult<TransacaoDto>.Fail(erroFinalidade);

            // Atualiza os campos
            transacaoExistente.Descricao = model.Descricao;
            transacaoExistente.Valor = model.Valor;
            transacaoExistente.Tipo = model.Tipo;
            transacaoExistente.DataTransacao = model.DataTransacao;
            transacaoExistente.CategoriaId = model.CategoriaId;
            transacaoExistente.PessoaId = model.PessoaId;

            await transacoesRepository.UpdateAsync(transacaoExistente, cancellationToken);
            return ApiResult<TransacaoDto>.Ok(mapper.Map<Transacao, TransacaoDto>(transacaoExistente));
        }
        catch
        {
            return ApiResult<TransacaoDto>.Fail("Ocorreu um erro ao atualizar a transação");
        }
    }

    // Retorna transacoes paginadas com filtros opcionais.
    public async Task<ApiResult<PagedResultDto<TransacaoDto>>> GetPaginateAsync(
        int skip = 0, 
        int take = 20, 
        DateTime? dataInicio = null, 
        DateTime? dataFim = null,
        int? pessoaId = null,
        int? categoriaId = null,
        TipoTransacao? tipo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Garante valores seguros para paginação
            var safeSkip = Math.Max(0, skip);
            var safeTake = Math.Max(1, take);

            // Cria expressão de filtro where combinando todos os filtros
            Expression<Func<Transacao, bool>>? where = t =>
                (!dataInicio.HasValue || t.DataTransacao.Date >= dataInicio.Value.Date) &&
                (!dataFim.HasValue || t.DataTransacao.Date <= dataFim.Value.Date) &&
                (!pessoaId.HasValue || t.PessoaId == pessoaId.Value) &&
                (!categoriaId.HasValue || t.CategoriaId == categoriaId.Value) &&
                (!tipo.HasValue || t.Tipo == tipo.Value);

            // Ordena por data
            Expression<Func<Transacao, object>> orderBy = t => t.DataTransacao;

            // Busca os itens paginados com as relações incluídas
            var transacoes = await transacoesRepository.PaginateWithIncludesAsync(
                safeSkip, 
                safeTake, 
                where, 
                orderBy, 
                OrderDirection.Descending, 
                cancellationToken);

            var totalItems = await transacoesRepository.CountAsync(where, cancellationToken);

            // Mapeia os DTOs
            var mapped = transacoes.Select(t => mapper.Map<Transacao, TransacaoDto>(t)).ToList();

            // Calcula informações de paginação
            var currentPage = (safeSkip / safeTake) + 1;
            var totalPages = (int)Math.Ceiling(totalItems / (double)safeTake);
            var hasPreviousPage = currentPage > 1;
            var hasNextPage = currentPage < totalPages;

            var result = new PagedResultDto<TransacaoDto>
            {
                Items = mapped,
                CurrentPage = currentPage,
                PageSize = safeTake,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = hasPreviousPage,
                HasNextPage = hasNextPage,
            };

            return ApiResult<PagedResultDto<TransacaoDto>>.Ok(result);
        }
        catch
        {
            return ApiResult<PagedResultDto<TransacaoDto>>.Fail("Ocorreu um erro ao listar transações");
        }
    }

    // Busca uma transação por id.
    public async Task<ApiResult<TransacaoDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var transacao = await transacoesRepository.GetByIdAsync(id, cancellationToken);
            
            if (transacao is null)
                return ApiResult<TransacaoDto>.Fail("Transação não encontrada");

            return ApiResult<TransacaoDto>.Ok(mapper.Map<Transacao, TransacaoDto>(transacao));
        }
        catch
        {
            return ApiResult<TransacaoDto>.Fail("Ocorreu um erro ao buscar a transação");
        }
    }

    // Remove uma transação, se existir.
    public async Task<ApiResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var transacao = await transacoesRepository.GetByIdAsync(id, cancellationToken);
            if (transacao is null)
                return ApiResult<bool>.Fail("Transação não encontrada");

            await transacoesRepository.DeleteAsync(transacao, cancellationToken);
            return ApiResult<bool>.Ok(true);
        }
        catch
        {
            return ApiResult<bool>.Fail("Ocorreu um erro ao excluir a transação");
        }
    }

    // Retorna os totais gerais de transações com filtros opcionais
    public async Task<ApiResult<TotaisGeraisDto>> GetTotaisGeraisAsync(
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        int? pessoaId = null,
        int? categoriaId = null,
        TipoTransacao? tipo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Calcula totais usando o repository com filtros
            // Se tipo for fornecido, calcula apenas aquele tipo, senão calcula ambos
            decimal totalReceitas;
            decimal totalDespesas;

            if (tipo.HasValue)
            {
                // Se tipo for fornecido, calcula apenas aquele tipo
                if (tipo.Value == TipoTransacao.Receita)
                {
                    totalReceitas = await transacoesRepository.SomarTransacoesPorTipoAsync(
                        TipoTransacao.Receita, pessoaId, categoriaId, dataInicio, dataFim, cancellationToken);
                    totalDespesas = 0;
                }
                else
                {
                    totalReceitas = 0;
                    totalDespesas = await transacoesRepository.SomarTransacoesPorTipoAsync(
                        TipoTransacao.Despesa, pessoaId, categoriaId, dataInicio, dataFim, cancellationToken);
                }
            }
            else
            {
                // Calcula ambos os tipos
                totalReceitas = await transacoesRepository.SomarTransacoesPorTipoAsync(
                    TipoTransacao.Receita, pessoaId, categoriaId, dataInicio, dataFim, cancellationToken);
                totalDespesas = await transacoesRepository.SomarTransacoesPorTipoAsync(
                    TipoTransacao.Despesa, pessoaId, categoriaId, dataInicio, dataFim, cancellationToken);
            }

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

    private static string? ValidarFinalidade(TipoTransacao tipo, FinalidadeCategoria finalidadeCategoria)
    {
        if (tipo == TipoTransacao.Despesa && finalidadeCategoria == FinalidadeCategoria.Receita)
            return "Categoria de receita não pode ser usada em despesa";

        if (tipo == TipoTransacao.Receita && finalidadeCategoria == FinalidadeCategoria.Despesa)
            return "Categoria de despesa não pode ser usada em receita";

        return null;
    }
}

