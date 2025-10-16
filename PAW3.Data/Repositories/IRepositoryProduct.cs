using PAW3.Data.Models;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace PAW3.Data.Repositories
{
    public interface IRepositoryProduct
    {
        Task<Product?> FindAsync(int id, CancellationToken ct = default);
        Task<List<Product>> ReadAsync(Expression<Func<Product, bool>>? filter = null, CancellationToken ct = default);
        Task AddAsync(Product entity, CancellationToken ct = default);
        void Update(Product entity);
        void Remove(Product entity);
        Task<bool> ExistsAsync(int productId, CancellationToken ct = default);
    }
}