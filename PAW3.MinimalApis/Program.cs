using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProductDb>(opt => opt.UseInMemoryDatabase("ProductList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/productitems", async (ProductDb db) =>
    await db.Products.ToListAsync());

app.MapGet("/productitems/complete", async (ProductDb db) =>
    await db.Products.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/productitems/{id}", async (int id, ProductDb db) =>
    await db.Products.FindAsync(id)
        is Product product
            ? Results.Ok(product)
            : Results.NotFound());

app.MapPost("/productitems", async (Product product, ProductDb db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/productitems/{product.Id}", product);
});

app.MapPut("/productitems/{id}", async (int id, Product inputProduct, ProductDb db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product is null) return Results.NotFound();

    product.Name = inputProduct.Name;
    product.IsComplete = inputProduct.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/productitems/{id}", async (int id, ProductDb db) =>
{
    if (await db.Products.FindAsync(id) is Product product)
    {
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();