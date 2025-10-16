using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PAW3.Data.Models;
using Task = System.Threading.Tasks.Task;

namespace PAW3.Data.Repositories
{
    /* IMPLEMENTACION DE REPOSITORY PATTERN
      
     Este repositorio encapsula la logica de acceso a datos para la entidad Product,
     proporcionando una interfaz limpia. Las operaciones de creacion, actualizacion 
     o eliminacion no confirman los cambios directamente en la base de datos: eso 
     lo gestiona la capa Unit of Work (IUnitOfWork), permitiendo agrupar varias 
     operaciones en una sola transaccion. */

    public class RepositoryProduct : IRepositoryProduct
    {
        private readonly ProductDbContext _db;

        public RepositoryProduct(ProductDbContext db)
        {
            _db = db;
        }

        /// Busca un producto por su ID.
        public Task<Product?> FindAsync(int id, CancellationToken ct = default)
        {
            return _db.Products.FirstOrDefaultAsync(p => p.ProductId == id, ct);
        }

        /// Obtiene una lista de productos, con un filtro opcional.
        public async Task<List<Product>> ReadAsync(
            Expression<Func<Product, bool>>? filter = null,
            CancellationToken ct = default)
        {
            var query = _db.Products.AsNoTracking().AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync(ct);
        }

        /// Agrega una nueva entidad al contexto.
        public Task AddAsync(Product entity, CancellationToken ct = default)
        {
            return _db.Products.AddAsync(entity, ct).AsTask();
        }

        /// Marca una entidad como modificada.
        public void Update(Product entity)
        {
            _db.Products.Update(entity);
        }

        /// Marca una entidad para eliminacion.
        public void Remove(Product entity)
        {
            _db.Products.Remove(entity);
        }

        /// Verifica si un producto existe por ID.
        public Task<bool> ExistsAsync(int productId, CancellationToken ct = default)
        {
            return _db.Products.AnyAsync(x => x.ProductId == productId, ct);
        }
    }
}

/*
   Explicacion del Patron Repository:

   Donde se implementa:

      El patron Repository se implementa en esta clase "RepositoryProduct", 
      la cual actua como intermediario entre la capa de negocio (Core) 
      y la base de datos (Data). Se encarga de gestionar el acceso a los 
      datos de la entidad Product mediante metodos genericos y expresivos, 
      sin que la capa superior necesite conocer detalles del SQL.

   Por que se eligio:

      Elegi este patron porque mejora la separacion de capas, 
      facilita las pruebas unitarias y hace que la logica de negocio este 
      desacoplada del acceso a datos directo con Entity Framework. Asi mismo,
      acceso logico de los datos esta centralizado en un solo lugar (Repository)
*/