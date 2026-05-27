using EcommerceAPI.Models;
using Scalar.AspNetCore;
using EcommerceAPI.Repositories;
using EcommerceAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/products", async (IProductRepository repository) =>
{
    var products = await repository.GetAllAsync();
    return Results.Ok(products);
});

app.MapGet("/products/{id:int}", async (int id, IProductRepository repository) =>
{
    var product = await repository.GetByIdAsync(id);
    return product is null 
        ? Results.NotFound(new { message = $"Product with ID {id} was not found." }) 
        : Results.Ok(product);
});

app.MapPost("/products", async (CreateProductDto dto, IProductRepository repository) =>
{
    await repository.CreateAsync(dto);
    return Results.Created($"/products", "Product created successfully!");
});

app.MapPut("/products/{id:int}", async (int id, CreateProductDto dto, IProductRepository repository) =>
{
    var updated = await repository.UpdateAsync(id, dto);
    return updated 
        ? Results.Ok(new { message = "Product updated successfully!" }) 
        : Results.NotFound(new { message = $"Cannot update. Product with ID {id} does not exist." });
});

app.MapDelete("/products/{id:int}", async (int id, IProductRepository repository) =>
{
    var deleted = await repository.DeleteAsync(id);
    return deleted 
        ? Results.Ok(new { message = "Product deleted successfully!" }) 
        : Results.NotFound(new { message = $"Cannot delete. Product with ID {id} was not found." });
});

app.MapGet("/categories", async (ICategoryRepository repository) =>
{
    var categories = await repository.GetAllAsync();
    return Results.Ok(categories);
});

app.MapGet("/categories/{id:int}", async (int id, ICategoryRepository repository) =>
{
    var category = await repository.GetByIdAsync(id);
    return category is null 
        ? Results.NotFound(new { message = $"Category with ID {id} was not found." }) 
        : Results.Ok(category);
});

app.MapPost("/categories", async (Category category, ICategoryRepository repository) =>
{
    await repository.CreateAsync(category);
    return Results.Created($"/categories", "Category created successfully!");
});

app.MapPut("/categories/{id:int}", async (int id, Category category, ICategoryRepository repository) =>
{
    var updated = await repository.UpdateAsync(id, category);
    return updated 
        ? Results.Ok(new { message = "Category updated successfully!" }) 
        : Results.NotFound(new { message = $"Cannot update. Category with ID {id} does not exist." });
});

app.MapDelete("/categories/{id:int}", async (int id, ICategoryRepository repository) =>
{
    var deleted = await repository.DeleteAsync(id);
    return deleted 
        ? Results.Ok(new { message = "Category deleted successfully!" }) 
        : Results.NotFound(new { message = $"Cannot delete. Category with ID {id} was not found." });
});

app.Run();