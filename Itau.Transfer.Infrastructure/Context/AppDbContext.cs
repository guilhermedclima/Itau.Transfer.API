using Itau.Transfer.Domain.Entities;
using Itau.Transfer.Infrastructure.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Itau.Transfer.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Transferencia> Transferencias { get; set; } = null;
  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TransferenciaConfiguration());
    }
}