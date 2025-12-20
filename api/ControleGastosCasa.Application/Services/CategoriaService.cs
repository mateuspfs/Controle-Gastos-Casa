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

public class CategoriaService(IGenericRepository<Categoria> categoriasRepository, IMapper mapper) : ICategoriaService
{
    // Cria uma categoria.
    public async Task<ApiResult<CategoriaDto>> CreateAsync(CategoriaDto model, CancellationToken cancellationToken = default)
    {
        try
        {
            var categoria = mapper.Map<CategoriaDto, Categoria>(model);

            await categoriasRepository.AddAsync(categoria, cancellationToken);
            return ApiResult<CategoriaDto>.Ok(mapper.Map<Categoria, CategoriaDto>(categoria));
        }
        catch
        {
            return ApiResult<CategoriaDto>.Fail("Ocorreu um erro ao criar a categoria");
        }
    }

    // Atualiza uma categoria existente
    public async Task<ApiResult<CategoriaDto>> UpdateAsync(int id, CategoriaDto model, CancellationToken cancellationToken = default)
    {
        try
        {
            var categoria = await categoriasRepository.GetByIdAsync(id, cancellationToken);
            if (categoria is null)
                return ApiResult<CategoriaDto>.Fail("Categoria não encontrada");

            mapper.Map(model, categoria);
            await categoriasRepository.UpdateAsync(categoria, cancellationToken);
            
            return ApiResult<CategoriaDto>.Ok(mapper.Map<Categoria, CategoriaDto>(categoria));
        }
        catch(Exception e)
        {
            return ApiResult<CategoriaDto>.Fail("Ocorreu um erro ao atualizar a categoria");
        }
    }

    // Retorna categorias paginadas com filtro opcional por descrição e finalidade
    public async Task<ApiResult<PagedResultDto<CategoriaDto>>> GetAllAsync(int skip = 0, int take = 20, string? searchTerm = null, int? finalidade = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Garante valores seguros para paginação
            var safeSkip = Math.Max(0, skip);
            var safeTake = Math.Max(1, take);

            // Normaliza o termo de busca (remove espaços extras)
            var normalizedSearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();

            // Cria expressão de filtro where combinando searchTerm e finalidade
            Expression<Func<Categoria, bool>>? where = null;
            
            if (!string.IsNullOrWhiteSpace(normalizedSearchTerm) && finalidade.HasValue)
            {
                var finalidadeValue = (FinalidadeCategoria)finalidade.Value;
                where = c => c.Descricao.Contains(normalizedSearchTerm) && c.Finalidade == finalidadeValue;
            }
            else if (!string.IsNullOrWhiteSpace(normalizedSearchTerm))
            {
                where = c => c.Descricao.Contains(normalizedSearchTerm);
            }
            else if (finalidade.HasValue)
            {
                var finalidadeValue = (FinalidadeCategoria)finalidade.Value;
                where = c => c.Finalidade == finalidadeValue;
            }

            // Ordena por ID descendente (últimos registros primeiro)
            Expression<Func<Categoria, object>> orderBy = c => c.Id;

            // Busca os itens paginados e o total de registros sequencialmente
            var categorias = await categoriasRepository.PaginateAsync(safeSkip, safeTake, where, orderBy, cancellationToken);
            var totalItems = await categoriasRepository.CountAsync(where, cancellationToken);

            // Mapeia os DTOs
            var mapped = categorias.Select(c => mapper.Map<Categoria, CategoriaDto>(c)).ToList();

            // Calcula informações de paginação
            var currentPage = (safeSkip / safeTake) + 1;
            var totalPages = (int)Math.Ceiling(totalItems / (double)safeTake);
            var hasPreviousPage = currentPage > 1;
            var hasNextPage = currentPage < totalPages;

            var result = new PagedResultDto<CategoriaDto>
            {
                Items = mapped,
                CurrentPage = currentPage,
                PageSize = safeTake,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = hasPreviousPage,
                HasNextPage = hasNextPage,
            };

            return ApiResult<PagedResultDto<CategoriaDto>>.Ok(result);
        }
        catch
        {
            return ApiResult<PagedResultDto<CategoriaDto>>.Fail("Ocorreu um erro ao listar categorias");
        }
    }

    // Busca categoria por id.
    public async Task<ApiResult<CategoriaDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var categoria = await categoriasRepository.GetByIdAsync(id, cancellationToken);
            if (categoria is null)
                return ApiResult<CategoriaDto>.Fail("Categoria não encontrada");

            return ApiResult<CategoriaDto>.Ok(mapper.Map<Categoria, CategoriaDto>(categoria));
        }
        catch
        {
            return ApiResult<CategoriaDto>.Fail("Ocorreu um erro ao buscar a categoria");
        }
    }
}

