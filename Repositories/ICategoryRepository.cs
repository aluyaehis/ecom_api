using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task CreateAsync(Category category);
    Task<bool> UpdateAsync(int id, Category category);
    Task<bool> DeleteAsync(int id);
}