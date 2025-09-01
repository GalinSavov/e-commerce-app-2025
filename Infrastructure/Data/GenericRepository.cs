using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class GenericRepository<T>(StoreContext storeContext) : IGenericRepository<T> where T : BaseEntity
{
    public void Create(T entity)
    {
        storeContext.Set<T>().Add(entity);
    }

    public void Delete(T entity)
    {
        storeContext.Set<T>().Remove(entity);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        var entities = await storeContext.Set<T>().ToListAsync();
        return entities;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var entity = await storeContext.Set<T>().FindAsync(id);
        return entity;
    }

    public async Task<bool> ProductExists(int id)
    {
        return await storeContext.Set<T>().AnyAsync(x => x.Id == id);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await storeContext.SaveChangesAsync() > 0;
    }

    public void Update(T entity)
    {
        storeContext.Set<T>().Attach(entity);
        storeContext.Entry(entity).State = EntityState.Modified;
    }
}
