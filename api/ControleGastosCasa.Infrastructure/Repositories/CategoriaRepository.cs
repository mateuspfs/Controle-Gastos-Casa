using ControleGastosCasa.Domain.Entities;
using ControleGastosCasa.Infrastructure.Persistence;
using ControleGastosCasa.Infrastructure.Repositories.Interfaces;

namespace ControleGastosCasa.Infrastructure.Repositories;

public class CategoriaRepository(AppDbContext context) : GenericRepository<Categoria>(context), ICategoriaRepository;

