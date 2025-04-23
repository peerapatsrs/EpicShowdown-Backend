using EpicShowdown.API.Models;

namespace EpicShowdown.API.Data.Base;

public interface IRepositoryBase<T> where T : Entity<int>
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangesAsync();
}