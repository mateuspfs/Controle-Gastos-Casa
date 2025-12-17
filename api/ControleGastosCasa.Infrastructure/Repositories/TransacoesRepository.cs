using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Infrastructure.Persistence;

namespace ControleGastosCasa.Infrastructure.Repositories;

public class TransacoesRepository(AppDbContext context) : GenericRepository<Transacao>(context);

