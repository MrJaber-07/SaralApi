using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaralApi.DTOs.Products;
using SaralApi.Models;

namespace SaralApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext context;
        public ProductController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpPost]
        public IActionResult CreateProduct(ProductRequest request)
        {
            // 1. Map the DTO to the Entity
            var newProduct = new Product
            {
                Name = request.Name,
                Stock = request.Stock
                // Note: Do NOT set RowVersion here, the DB handles it
            };

            // 2. Add the Entity, NOT the request
            context.Products.Add(newProduct);

            context.SaveChanges();

            return Ok(newProduct);
        }
    }
}
