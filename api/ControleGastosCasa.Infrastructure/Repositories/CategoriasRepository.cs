using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Infrastructure.Persistence;

namespace ControleGastosCasa.Infrastructure.Repositories;

public class CategoriasRepository(AppDbContext context) : GenericRepository<Categoria>(context);

