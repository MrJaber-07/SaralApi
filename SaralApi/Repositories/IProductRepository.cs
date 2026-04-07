using SaralApi.Models;

namespace SaralApi.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetProductWithLockAsync(int id);
        Task SaveChangesAsync();
    }
}
