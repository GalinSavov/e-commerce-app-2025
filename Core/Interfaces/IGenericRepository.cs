using Core.Entities;
namespace Core.Interfaces;

//Generic repository interface that all other repositories will implement and inherit from
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    void Create(T entity);
    void Delete(T entity);
    void Update(T entity);
    Task<bool> SaveAllAsync();
    Task<bool> ProductExists(int id);
}
