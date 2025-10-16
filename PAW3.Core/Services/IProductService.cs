using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PAW3.Models.DTOs;

namespace PAW3.Core.Services
{
    public interface IProductService
    {
        Task<List<ProductDTO>> ListAsync(string? q = null, CancellationToken ct = default);
        Task<ProductDTO?> GetAsync(int id, CancellationToken ct = default);
        Task<ProductDTO> CreateAsync(ProductDTO dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, ProductDTO dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}