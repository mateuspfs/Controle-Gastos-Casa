using System.Linq.Expressions;

namespace ControleGastosCasa.Infrastructure.Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> PaginateAsync(int skip = 0, int take = 20, Expression<Func<T, bool>>? where = null, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? where = null, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}

