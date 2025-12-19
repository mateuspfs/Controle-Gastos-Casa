using System;
using System.Linq;
using ControleGastosCasa.Domain.Repositories;
using ControleGastosCasa.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ControleGastosCasa.Infrastructure.Repositories;

/// <summary>
/// Implementação base usando EF Core (PostgreSQL).
/// </summary>
/// <typeparam name="T">Tipo de entidade de domínio.</typeparam>
public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : class
{
    protected AppDbContext Context { get; } = context;

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().FindAsync([id], cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> PaginateAsync(int skip = 0, int take = 20, CancellationToken cancellationToken = default)
    {
        var safeSkip = Math.Max(0, skip);
        var safeTake = Math.Max(1, take);
        return await Context.Set<T>()
            .AsNoTracking()
            .Skip(safeSkip)
            .Take(safeTake)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<T>().AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<T>().FindAsync([id], cancellationToken) is not null;
    }
}

