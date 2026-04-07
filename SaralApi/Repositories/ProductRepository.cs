using SaralApi.Models;
//using SaralApi.Data; // Ensure this is the correct namespace for your AppDbContext

namespace SaralApi.Repositories
{
    // Inheriting from Repository<Product> automatically implements GetAllAsync, AddAsync, and Delete
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        // Implementation for the specific IProductRepository member
        public async Task<Product?> GetProductWithLockAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}