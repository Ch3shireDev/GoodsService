namespace GoodsService.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetAsync(Guid id);
    Task<IEnumerable<TEntity>> ListAsync(int pageSize, int page);
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity?> UpdateAsync(TEntity entity);
    Task DeleteAsync(Guid id);
    Task<bool> Exists(Guid id);
}
