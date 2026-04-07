namespace SaralApi.Models
{
    public enum OrderStatus { INIT, PROCESSING, SUCCESS, FAILED, ROLLED_BACK }
    public class Order
    {
        public int Id { get; set; }
        public string IdempotencyKey { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<OrderItem> Items { get; set; } = new();
    }
}
