using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using PAW3.Data.Models;   // DbContext + Product
using PAW3.Models.DTOs;   // ProductDTO

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS para MVC
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("PAW3Client", p => p
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
products.MapGet("/", async (ProductDbContext db) =>
{
    var list = await db.Products
        .AsNoTracking()
        .Select(ProductMap.ToDto)   // ?? expresión estática traducible por EF
        .ToListAsync();

    return TypedResults.Ok(list);
});

// GET by id
products.MapGet("/{id:int}", async Task<Results<Ok<ProductDTO>, NotFound>> (int id, ProductDbContext db) =>
{
    var dto = await db.Products
        .AsNoTracking()
        .Where(x => x.ProductId == id)
        .Select(ProductMap.ToDto)   // ??
        .FirstOrDefaultAsync();

    if (dto is null) return TypedResults.NotFound();
    return TypedResults.Ok(dto);
});

// POST
products.MapPost("/", async Task<Results<Created<ProductDTO>, BadRequest<string>>> (ProductDbContext db, ProductDTO dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.ProductName))
        return TypedResults.BadRequest("productName is required.");

    var entity = new Product();
    ProductMap.AplicarDTO(entity, dto, isUpdate: false);

    db.Products.Add(entity);
    await db.SaveChangesAsync();

    var createdDto = ProductMap.FromEntity(entity);
    return TypedResults.Created($"/api/products/{entity.ProductId}", createdDto);
});

// PUT
products.MapPut("/{id:int}", async Task<Results<NoContent, NotFound, BadRequest<string>>> (int id, ProductDbContext db, ProductDTO dto) =>
{
    var entity = await db.Products.FirstOrDefaultAsync(x => x.ProductId == id);
    if (entity is null) return TypedResults.NotFound();

    ProductMap.AplicarDTO(entity, dto, isUpdate: true);
    await db.SaveChangesAsync();

    return TypedResults.NoContent();
});

// DELETE
products.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (int id, ProductDbContext db) =>
{
    var entity = await db.Products.FirstOrDefaultAsync(x => x.ProductId == id);
    if (entity is null) return TypedResults.NotFound();

    db.Products.Remove(entity);
    await db.SaveChangesAsync();

    return TypedResults.NoContent();
});

app.MapGet("/", () => "PAW3 Minimal API is running");
app.Run();
internal static class ProductMap
{
    // Ttraduce por EF en .Select(), hace los queries en SQL
    public static readonly Expression<Func<Product, ProductDTO>> ToDto = p => new ProductDTO
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