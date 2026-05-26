using MySqlConnector;
using Dapper;
using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "SELECT id, name, description, price, stock_quantity AS StockQuantity, category_id AS CategoryId FROM Products";
        return await connection.QueryAsync<Product>(sql);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "SELECT id, name, description, price, stock_quantity AS StockQuantity, category_id AS CategoryId FROM Products WHERE id = @Id";
        return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task CreateAsync(CreateProductDto dto)
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "INSERT INTO Products (name, description, price, stock_quantity, category_id) VALUES(@Name, @Description, @Price, @StockQuantity, @CategoryId);";
        await connection.ExecuteAsync(sql, new { dto.Name, dto.Description, dto.Price, dto.StockQuantity, dto.CategoryId });
    }

    public async Task<bool> UpdateAsync(int id, CreateProductDto dto)
    {
        using var connection = new MySqlConnection(_connectionString);
        string checkSql = "SELECT COUNT(1) FROM Products WHERE id = @Id";
        var exists = await connection.ExecuteScalarAsync<bool>(checkSql, new { Id = id });

        if(!exists) return false;

        string updateSql = @"UPDATE Products 
                             SET name = @Name, description = @Description, price = @Price, 
                                 stock_quantity = @StockQuantity, category_id = @CategoryId 
                             WHERE id = @Id;";

        await connection.ExecuteAsync(updateSql, new { 
            Id = id, dto.Name, dto.Description, dto.Price, dto.StockQuantity, dto.CategoryId 
        });
        
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        string sql = "DELETE FROM Products WHERE id = @Id";
        int rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}