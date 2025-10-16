using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProductDb>(opt => opt.UseInMemoryDatabase("ProductList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

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
{
    var product = await db.Products.FindAsync(id);

    if (product is null) return TypedResults.NotFound();

    product.Name = productItemDTO.Name;
    product.IsComplete = productItemDTO.IsComplete;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteProduct(int id, ProductDb db)
{
    if (await db.Products.FindAsync(id) is Product product)
    {
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}