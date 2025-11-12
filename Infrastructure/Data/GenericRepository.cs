using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
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

    public async Task<IReadOnlyList<T>> ListAllAsync()
    {
        var entities = await storeContext.Set<T>().ToListAsync();
        return entities;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var entity = await storeContext.Set<T>().FindAsync(id);
        return entity;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(ISpecification<T> specification)
    {
        var query = ApplySpecification(specification);
        return await query.ToListAsync();
    }

    public async Task<T?> GetEntityWithSpec(ISpecification<T> specification)
    {
        var query = ApplySpecification(specification);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<bool> ProductExists(int id)
    {
        return await storeContext.Set<T>().AnyAsync(x => x.Id == id);
    }
    public void Update(T entity)
    {
        storeContext.Set<T>().Attach(entity);
        storeContext.Entry(entity).State = EntityState.Modified;
    }
    private IQueryable<T> ApplySpecification(ISpecification<T> specification)
    {
        return SpecificationEvaluator<T>.Evaluate(storeContext.Set<T>().AsQueryable(), specification);
    }
     private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T,TResult> specification)
    {
        return SpecificationEvaluator<T>.Evaluate<T,TResult>(storeContext.Set<T>().AsQueryable(), specification);
    }

    public async Task<TResult?> GetEntityWithSpec<TResult>(ISpecification<T, TResult> specification)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<TResult>> GetAllAsync<TResult>(ISpecification<T, TResult> specification)
    {
        return await ApplySpecification(specification).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<T> specification)
    {
        var query = storeContext.Set<T>().AsQueryable();
        specification.ApplyCriteria(query);
        return await query.CountAsync();
    }
}
