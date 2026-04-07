using Microsoft.AspNetCore.Mvc;
using SaralApi.DTOs.Products;
using SaralApi.Models;
using SaralApi.Repositories;

namespace SaralApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepo;

        public ProductController(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        // 1. CREATE: Add a new product
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
        {
            var newProduct = new Product
            {
                Name = request.Name,
                Stock = request.Stock
                // RowVersion is handled automatically by the DB
            };

            await _productRepo.AddAsync(newProduct);
            await _productRepo.SaveChangesAsync();

            return Ok(newProduct);
        }

        // 2. READ (ALL): Get all products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepo.GetAllAsync();
            return Ok(products);
        }

        // 3. READ (SINGLE): Get a product by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound($"Product with ID {id} not found.");

            return Ok(product);
        }

        // 4. UPDATE: Modify product details or manually adjust stock
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            if (id != updatedProduct.Id) return BadRequest("ID mismatch.");

            var existingProduct = await _productRepo.GetByIdAsync(id);
            if (existingProduct == null) return NotFound();

            // Update fields
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Stock = updatedProduct.Stock;

            _productRepo.Update(existingProduct);
            await _productRepo.SaveChangesAsync();

            return NoContent();
        }

        // 5. DELETE: Remove a product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();

            _productRepo.Delete(product);
            await _productRepo.SaveChangesAsync();

            return Ok(new { Message = $"Product {id} deleted successfully." });
        }
    }
}