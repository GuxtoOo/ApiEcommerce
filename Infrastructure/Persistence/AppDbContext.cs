using ApiEcommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public AppDbContext(DbContextOptions<AppDbContext> o) : base(o) { }
    protected override void OnModelCreating(ModelBuilder b)
    { b.Entity<Order>(o => { o.HasKey(x => x.Id); o.OwnsMany(x => x.Items); }); }
}
