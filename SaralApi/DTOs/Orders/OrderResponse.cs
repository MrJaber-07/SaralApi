namespace SaralApi.DTOs.Orders
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
