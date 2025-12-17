using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Infrastructure.Persistence;

namespace ControleGastosCasa.Infrastructure.Repositories;

public class PessoasRepository(AppDbContext context) : GenericRepository<Pessoa>(context);

