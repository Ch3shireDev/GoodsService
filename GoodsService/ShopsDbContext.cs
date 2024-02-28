using GoodsService.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodsService;

public class ShopsDbContext(DbContextOptions<ShopsDbContext> options) : DbContext(options)
{
    public DbSet<ShopEntity> Shops { get; set; }
    public DbSet<GoodsEntity> Goods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<ShopEntity>()
            .HasMany(s => s.Goods)
            .WithOne()
            .HasForeignKey(g => g.ShopId)
        ;
    }

}