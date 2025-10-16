using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PAW3.Data.Repositories;
using PAW3.Core.Services;         
using PAW3.Data.Models;            
using PAW3.Models.DTOs;            

namespace PAW3.Core.Services
{
    /// IMPLEMENTACION DE SERVICE LAYER PATTERN
    /// Implementacion del patron Service Layer para Product.
    /// Trabaja con los patrones Repository y UnitOfWork, mapea DTOs y aplica reglas de negocio.


    public class ProductService : IProductService
    {
        private readonly IRepositoryProduct _repo;
        private readonly IUnitOfWork _uow;

        public ProductService(IRepositoryProduct repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        //  Mapeo (DTO <-> Entity)
        private static ProductDTO ToDto(Product p) => new ProductDTO
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

        private static void ApplyDto(Product target, ProductDTO src, bool isUpdate)
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

        // API del Servicio 
        public async Task<List<ProductDTO>> ListAsync(string? q = null, CancellationToken ct = default)
        {
            var list = await _repo.ReadAsync(null, ct);

            if (!string.IsNullOrWhiteSpace(q))
                list = list.Where(p => p.ProductName != null && p.ProductName.Contains(q)).ToList();

            return list.Select(ToDto).ToList();
        }

        public async Task<ProductDTO?> GetAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.FindAsync(id, ct);
            return entity is null ? null : ToDto(entity);
        }

        public async Task<ProductDTO> CreateAsync(ProductDTO dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.ProductName))
                throw new ArgumentException("productName is required.", nameof(dto));

            var entity = new Product();
            ApplyDto(entity, dto, isUpdate: false);

            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            return ToDto(entity);
        }

        public async Task<bool> UpdateAsync(int id, ProductDTO dto, CancellationToken ct = default)
        {
            var entity = await _repo.FindAsync(id, ct);
            if (entity is null) return false;

            ApplyDto(entity, dto, isUpdate: true);
            _repo.Update(entity);

            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.FindAsync(id, ct);
            if (entity is null) return false;

            _repo.Remove(entity);
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }
}

/* 
        Explicacion del Patron Service Layer:

   Donde se implementa:
      En la clase "PAW3.Core.Services.ProductService", que expone operaciones
      de la aplicación en terminos de DTOs, trabajando el acceso a datos a traves
      de Repository ("IRepositoryProduct") y la confirmacion de las transacciones a traves
      de UnitOfWork ("IUnitOfWork"). Aqui tambien se encuentran las reglas de negocio
      (por ejemplo, setear LastModified del lado del servidor, validaciones, etc.).
    
    Por qué se eligio:

      Elegi Service Layer para definir un proceso claro de la aplicacion,
      mantener las reglas de negocio en un solo lugar, y simplificar
      controladores/endpoints, facilitando pruebas y mantenimiento. Separando
      los servicios y la logica del negocio, ya que trabaja como un intermediador
      entre el Controlador (API) y la capa de Datos.

*/