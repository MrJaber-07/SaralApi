using System.ComponentModel.DataAnnotations;

namespace SaralApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }

        [Timestamp] // Ensures atomic concurrency at the DB level
        public byte[]? RowVersion { get; set; }
    }
}
