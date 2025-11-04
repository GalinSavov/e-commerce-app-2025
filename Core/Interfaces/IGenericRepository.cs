using Core.Entities;
namespace Core.Interfaces;

//Generic repository interface that all other repositories will implement and inherit from
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetEntityWithSpec(ISpecification<T> specification);
    Task<IReadOnlyList<T>> GetAllAsync(ISpecification<T> specification);
    Task<TResult?> GetEntityWithSpec<TResult>(ISpecification<T, TResult> specification);
    Task<IReadOnlyList<TResult>> GetAllAsync<TResult>(ISpecification<T, TResult> specification);
    Task<IReadOnlyList<T>> ListAllAsync();
    void Create(T entity);
    void Delete(T entity);
    void Update(T entity);
    Task<bool> SaveAllAsync();
    Task<bool> ProductExists(int id);
    Task<int> CountAsync(ISpecification<T> specification);
}
