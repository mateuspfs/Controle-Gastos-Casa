using System.Linq.Expressions;
using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Infrastructure.Persistence;
using ControleGastosCasa.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ControleGastosCasa.Infrastructure.Repositories;

public class TransacaoRepository(AppDbContext context) : GenericRepository<Transacao>(context), ITransacaoRepository
{
    // Busca os itens paginados com as relações incluídas
    public async Task<IReadOnlyList<Transacao>> PaginateWithIncludesAsync(
        int skip = 0, 
        int take = 20, 
        Expression<Func<Transacao, bool>>? where = null, 
        Expression<Func<Transacao, object>>? orderBy = null, 
        OrderDirection orderDirection = OrderDirection.Descending, 
        CancellationToken cancellationToken = default)
    {
        var safeSkip = Math.Max(0, skip);
        var safeTake = Math.Max(1, take);
        
        var query = Context.Set<Transacao>()
            .AsNoTracking()
            .Include(t => t.Pessoa)
            .Include(t => t.Categoria)
            .AsQueryable();
        
        // Aplica filtro where se fornecido
        if (where != null) query = query.Where(where);
        
        // Aplica ordenação se fornecida
        if (orderBy != null)
            query = orderDirection == OrderDirection.Ascending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);
        
        return await query
            .Skip(safeSkip)
            .Take(safeTake)
            .ToListAsync(cancellationToken);
    }
}

