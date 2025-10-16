using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using PAW3.Data.Models;   // DbContext + Product
using PAW3.Models.DTOs;   // ProductDTO

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionDB")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("PAW3Client", policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("https://localhost:7181"));
});

var app = builder.Build();

<<<<<<< HEAD
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
        .Select(ProductMap.ConvertirDto)
        .ToListAsync();

    return TypedResults.Ok(list);
});

// GET by id
products.MapGet("/{id:int}", async Task<Results<Ok<ProductDTO>, NotFound>> (int id, ProductDbContext db) =>
=======
RouteGroupBuilder productItems = app.MapGroup("/productitems");

productItems.MapGet("/", GetAllProducts);
productItems.MapGet("/complete", GetCompleteProducts);
productItems.MapGet("/{id}", GetProduct);
productItems.MapPost("/", CreateProduct);
productItems.MapPut("/{id}", UpdateProduct);
productItems.MapDelete("/{id}", DeleteProduct);

app.Run();

static async Task<IResult> GetAllProducts(ProductDb db)
{
    return TypedResults.Ok(await db.Products.Select(x => new ProductItemDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetCompleteProducts(ProductDb db)
{
    return TypedResults.Ok(await db.Products.Where(t => t.IsComplete).Select(x => new ProductItemDTO(x)).ToListAsync());
}

static async Task<IResult> GetProduct(int id, ProductDb db)
{
    return await db.Products.FindAsync(id)
        is Product product
            ? TypedResults.Ok(new ProductItemDTO(product))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateProduct(ProductItemDTO productItemDTO, ProductDb db)
{
    var productItem = new Product
    {
        IsComplete = productItemDTO.IsComplete,
        Name = productItemDTO.Name
    };

    db.Products.Add(productItem);
    await db.SaveChangesAsync();

    productItemDTO = new ProductItemDTO(productItem);

    return TypedResults.Created($"/productitems/{productItem.Id}", productItemDTO);
}

static async Task<IResult> UpdateProduct(int id, ProductItemDTO productItemDTO, ProductDb db)
>>>>>>> main
{
    var dto = await db.Products
        .AsNoTracking()
        .Where(x => x.ProductId == id)
        .Select(ProductMap.ConvertirDto)
        .FirstOrDefaultAsync();

<<<<<<< HEAD
    if (dto is null) return TypedResults.NotFound();
    return TypedResults.Ok(dto);
});

// POST
products.MapPost("/", async Task<Results<Created<ProductDTO>, BadRequest<string>>> (ProductDbContext db, ProductDTO dto) =>
=======
    if (product is null) return TypedResults.NotFound();

    product.Name = productItemDTO.Name;
    product.IsComplete = productItemDTO.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteProduct(int id, ProductDb db)
>>>>>>> main
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

// Empiezan los Mappeos
internal static class ProductMap
{
    // Traducible por EF en .Select(), hace los Query en SQL
    public static readonly Expression<Func<Product, ProductDTO>> ConvertirDto = p => new ProductDTO
    {
<<<<<<< HEAD
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

    // Convierte Product a un DTO para poder utilizarse
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

    // Hace las actualizaciones a la base de datos, donde target es la base de datos y src es lo ingresado por el usuario en la interfaz
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
=======
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
>>>>>>> main
}