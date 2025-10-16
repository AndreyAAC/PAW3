using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PAW3.Core.Services;
using PAW3.Data.Models;
using PAW3.Data.Repositories;
using PAW3.Models.DTOs;
using System.Linq.Expressions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionDB")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Patron de Repository / Unit of Work / Service Layer
builder.Services.AddScoped<IRepositoryProduct, RepositoryProduct>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductService, ProductService>();


builder.Services.AddCors(opt =>
{
    opt.AddPolicy("PAW3Client", policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("https://localhost:7181"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PAW3Client");

var products = app.MapGroup("/api/products").WithTags("Products");

// GET all
products.MapGet("/", async (IProductService svc, string? q) =>
{
    var list = await svc.ListAsync(q);
    return TypedResults.Ok(list);
});

// GET by id
products.MapGet("/{id:int}", async Task<Results<Ok<ProductDTO>, NotFound>> (IProductService svc, int id) =>
{
    var dto = await svc.GetAsync(id);

    if (dto is null) return TypedResults.NotFound();
    return TypedResults.Ok(dto);
});

// POST
products.MapPost("/", async Task<Results<Created<ProductDTO>, BadRequest<string>>> (IProductService svc, ProductDTO dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.ProductName))
        return TypedResults.BadRequest("productName is required.");

    var createdDto = await svc.CreateAsync(dto);
    return TypedResults.Created($"/api/products/{createdDto.ProductId}", createdDto);
});

// PUT
products.MapPut("/{id:int}", async Task<Results<NoContent, NotFound, BadRequest<string>>> (IProductService svc, int id, ProductDTO dto) =>
{
    var ok = await svc.UpdateAsync(id, dto);
    return ok ? TypedResults.NoContent() : TypedResults.NotFound();
});

// DELETE
products.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (IProductService svc, int id) =>
{
    var ok = await svc.DeleteAsync(id);
    return ok ? TypedResults.NoContent() : TypedResults.NotFound();
});

app.MapGet("/", () => "PAW3 Minimal API is running");
app.Run();

internal static class ProductMap
{
    // Traduce por EF en .Select(), hace los queries en SQL
    public static readonly Expression<Func<Product, ProductDTO>> ConvertirDto = p => new ProductDTO
    {
        ProductId = p.ProductId,
        ProductName = p.ProductName,
        InventoryId = p.InventoryId,
        SupplierId = p.SupplierId,
        Description = p.Description,
        Rating = p.Rating,
        CategoryId = p.CategoryId,
        LastModified = p.LastModified,
        ModifiedBy = p.ModifiedBy
    };

    // Convierte Product a ProductDTO
    public static ProductDTO FromEntity(Product p) => new ProductDTO
    {
        ProductId = p.ProductId,
        ProductName = p.ProductName,
        InventoryId = p.InventoryId,
        SupplierId = p.SupplierId,
        Description = p.Description,
        Rating = p.Rating,
        CategoryId = p.CategoryId,
        LastModified = p.LastModified,
        ModifiedBy = p.ModifiedBy
    };

    // Hace cambios en la base de datos donde target es la BD y src es el input del user en la interfaz
    public static void AplicarDTO(Product target, ProductDTO src, bool isUpdate = false)
    {
        target.ProductName = src.ProductName;
        target.InventoryId = src.InventoryId;
        target.SupplierId = src.SupplierId;
        target.Description = src.Description;
        target.Rating = src.Rating;
        target.CategoryId = src.CategoryId;
        target.ModifiedBy = src.ModifiedBy;
        target.LastModified = DateTime.UtcNow;
    }
}