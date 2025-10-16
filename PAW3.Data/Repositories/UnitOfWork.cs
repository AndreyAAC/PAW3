using System.Threading;
using Task = System.Threading.Tasks.Task;
using PAW3.Data.Models;

namespace PAW3.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProductDbContext _db;
        public UnitOfWork(ProductDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);
    }
}