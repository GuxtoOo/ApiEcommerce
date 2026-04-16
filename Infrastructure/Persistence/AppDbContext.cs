using ApiEcommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public AppDbContext(DbContextOptions<AppDbContext> o) : base(o) { }
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Order>(o =>
        {
            o.ToTable("Orders");

            o.HasKey(x => x.Id);

            o.Property(x => x.Status)
                .HasConversion<int>();

            o.HasIndex(x => x.BuyerId);
            o.HasIndex(x => x.Status);

            o.OwnsMany(x => x.itens, i =>
            {
                i.ToTable("OrderItems");

                i.WithOwner().HasForeignKey("OrderId");

                i.Property<int>("Id");
                i.HasKey("Id");

                i.Property(p => p.ProductId).IsRequired();
                i.Property(p => p.Price).HasColumnType("decimal(18,2)");
                i.Property(p => p.Quantity).IsRequired();
                i.Property(p => p.TotalPrice).HasColumnType("decimal(18,2)");
            });
        });
    }
}
