using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Infrastructure.Persistence;

namespace ControleGastosCasa.Infrastructure.Repositories;

public class CategoriaRepository(AppDbContext context) : GenericRepository<Categoria>(context);

