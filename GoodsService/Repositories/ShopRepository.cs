using GoodsService.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodsService.Repositories;

public interface IShopRepository : IRepository<ShopEntity>
{
}

public class ShopRepository(ShopsDbContext context) : IShopRepository
{
    public async Task<ShopEntity?> GetAsync(Guid id)
    {
        return await context.FindAsync<ShopEntity>(id).ConfigureAwait(false);
    }

    public async Task<ShopEntity> AddAsync(ShopEntity entity)
    {
        context.Add(entity);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await context.FindAsync<ShopEntity>(id).ConfigureAwait(false);
        if (entity == null) return;
        context.Remove(entity);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return;
    }

    public async Task<IEnumerable<ShopEntity>> ListAsync(int pageSize, int page)
    {
        return await context.Shops
            .Skip(pageSize * page)
            .Take(pageSize)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<ShopEntity?> UpdateAsync(ShopEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var oldEntity = await context.FindAsync<ShopEntity>(entity.Id).ConfigureAwait(false);
        if (oldEntity == null) return null;
        context.Entry(oldEntity).CurrentValues.SetValues(entity);
        context.Entry(oldEntity).State = EntityState.Modified;

        await context.SaveChangesAsync().ConfigureAwait(false);

        return oldEntity;
    }

    public Task<bool> Exists(Guid id)
    {
        return context.Shops.AnyAsync(x => x.Id == id);
    }
}