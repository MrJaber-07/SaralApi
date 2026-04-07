using System.ComponentModel.DataAnnotations;

namespace SaralApi.DTOs.Products
{
    public class ProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }

        [Timestamp] // Ensures atomic concurrency at the DB level
        public byte[]? RowVersion { get; set; }
    }
}
