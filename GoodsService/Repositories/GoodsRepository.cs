using GoodsService.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodsService.Repositories;

public interface IGoodsRepository : IRepository<GoodsEntity>
{
}

public class GoodsRepository(ShopsDbContext context) : IGoodsRepository
{
    public async Task<GoodsEntity?> GetAsync(Guid id)
    {
        return await context.FindAsync<GoodsEntity>(id).ConfigureAwait(false);
    }

    public async Task<GoodsEntity> AddAsync(GoodsEntity entity)
    {
        await context.AddAsync(entity).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return entity;
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }


    public Task<IEnumerable<GoodsEntity>> ListAsync(int pageSize, int page)
    {
        throw new NotImplementedException();
    }

    public async Task<GoodsEntity?> UpdateAsync(GoodsEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var oldEntity = await context.Goods.FindAsync(entity.Id).ConfigureAwait(false);

        if (oldEntity == null) return null;

        context.Entry(oldEntity).CurrentValues.SetValues(entity);
        //context.Entry(oldEntity).State = EntityState.Modified;
        await context.SaveChangesAsync().ConfigureAwait(false);

        return entity;
    }

    public Task<bool> Exists(Guid id)
    {
        return context.Goods.AnyAsync(x => x.Id == id);
    }
}
