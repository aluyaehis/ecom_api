using MySqlConnector;
using Dapper;
using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly string _connectionString;

    public CategoryRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "SELECT id, name FROM Categories";
        return await connection.QueryAsync<Category>(sql);
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "SELECT id, name FROM Categories WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Id = id });
    }

    public async Task CreateAsync(Category category)
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "INSERT INTO Categories (name) VALUES (@Name);";
        await connection.ExecuteAsync(sql, category);
    }

    public async Task<bool> UpdateAsync(int id, Category category)
    {
        using var connection = new MySqlConnection(_connectionString);
        
        string checkSql = "SELECT COUNT(1) FROM Categories WHERE id = @Id";
        var exists = await connection.ExecuteScalarAsync<bool>(checkSql, new { Id = id });
        
        if (!exists) return false;

        string updateSql = "UPDATE Categories SET name = @Name WHERE id = @Id;";
        await connection.ExecuteAsync(updateSql, new { Id = id, category.Name });
        
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "DELETE FROM Categories WHERE id = @Id";
        int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}