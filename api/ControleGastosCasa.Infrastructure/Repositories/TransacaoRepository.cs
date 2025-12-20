using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Infrastructure.Persistence;
using ControleGastosCasa.Infrastructure.Repositories.Interfaces;

namespace ControleGastosCasa.Infrastructure.Repositories;

public class TransacaoRepository(AppDbContext context) : GenericRepository<Transacao>(context), ITransacaoRepository;

