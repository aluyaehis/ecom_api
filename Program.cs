using MySqlConnector;
using Dapper;
using EcommerceAPI.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

app.MapGet("/products", async () =>
{
    using var connection = new MySqlConnection(connectionString);

    string sql = "SELECT id, name, description, price, stock_quantity AS StockQuantity FROM Products";

    var products = await connection.QueryAsync<Product>(sql);

    return Results.Ok(products);
});

app.MapPost("/products", async (CreateProductDto dto) =>
{
    using var connection = new MySqlConnection(connectionString);
    string sql = @"INSERT INTO Products (name, description, price, stock_quantity, category_id) VALUES(@Name, @Description, @Price, @StockQuantity, @CategoryId)";

    await connection.ExecuteAsync(sql, dto);

    return Results.Created($"/products", "Product created successfully!");
});

app.MapPost("/categories", async (Category category) =>
{
    using var connection = new MySqlConnection(connectionString);
    
    string sql = "INSERT INTO Categories (name) VALUES (@Name);";
    
    await connection.ExecuteAsync(sql, category);
    
    return Results.Created("/categories", "Category created successfully!");
});

app.MapGet("/products/{id:int}", async(int id) =>
{
    using var connection = new MySqlConnection(connectionString);
    string sql = "SELECT id, name, description, price, stock_quantity AS StockQuantity, category_id AS CategoryId FROM Products where id = @Id";
    var product = await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });

    if(product == null)
    {
        return Results.NotFound(new { message = $"Product with ID {id} was not found." });
    }
    return Results.Ok(product);
});

app.MapPut("/products/{id:int}", async (int id, CreateProductDto dto) =>
{
    using var connection = new MySqlConnection(connectionString);
    string checkSql = "SELECT COUNT(1) FROM Products WHERE id = @Id";
    var exists = await connection.ExecuteScalarAsync<bool>(checkSql, new { Id = id });
    if (!exists)
    {
        return Results.NotFound(new { message = $"Cannot update. Product with ID {id} does not exist." });
    }

    string updateSql = @"UPDATE Products SET name = @Name, description = @Description, price = @Price, stock_quantity = @StockQuantity, category_id = @CategoryId WHERE id = @Id";
    await connection.ExecuteAsync(updateSql, new { Id = id,  dto.Name, dto.Description, dto.Price, dto.StockQuantity, dto.CategoryId });

    return Results.Ok(new { message = $"Product with ID {id} updated successfully!" });
});


app.MapDelete("/products/{id:int}", async(int id) =>
{
    using var connection = new MySqlConnection(connectionString);
    string sql = "DELETE FROM Products WHERE id = @Id";

    int rowAffected = await connection.ExecuteAsync(sql, new { Id = id });
    if(rowAffected == 0)
    {
        return Results.NotFound(new { message = $"Cannot delete. Product with ID {id} was not found." });  
    }
    return Results.Ok(new { message = "Product deleted successfully." });
});

app.Run();