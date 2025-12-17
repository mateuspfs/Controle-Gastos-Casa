using ControleGastosCasa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleGastosCasa.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Pessoa>(builder =>
        {
            builder.ToTable("pessoas");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Nome).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Idade).IsRequired();
        });

        modelBuilder.Entity<Categoria>(builder =>
        {
            builder.ToTable("categorias");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Descricao).IsRequired().HasMaxLength(150);
            builder.Property(c => c.Finalidade).IsRequired();
        });

        modelBuilder.Entity<Transacao>(builder =>
        {
            builder.ToTable("transacoes");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Descricao).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Valor).IsRequired().HasColumnType("numeric(18,2)");
            builder.Property(t => t.Tipo).IsRequired();

            builder.HasOne<Pessoa>()
                .WithMany()
                .HasForeignKey(t => t.PessoaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Categoria>()
                .WithMany()
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

