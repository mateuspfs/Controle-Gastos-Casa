using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Infrastructure.Persistence;

namespace ControleGastosCasa.Infrastructure.Repositories;

public class PessoaRepository(AppDbContext context) : GenericRepository<Pessoa>(context);

