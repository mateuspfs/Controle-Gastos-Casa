using System.Linq.Expressions;
using ControleGastosCasa.Domain.Entities;

namespace ControleGastosCasa.Infrastructure.Repositories.Interfaces;

public interface ITransacaoRepository : IGenericRepository<Transacao>
{
    Task<IReadOnlyList<Transacao>> PaginateWithIncludesAsync(
        int skip = 0, 
        int take = 20, 
        Expression<Func<Transacao, bool>>? where = null, 
        Expression<Func<Transacao, object>>? orderBy = null, 
        OrderDirection orderDirection = OrderDirection.Descending, 
        CancellationToken cancellationToken = default);
}

