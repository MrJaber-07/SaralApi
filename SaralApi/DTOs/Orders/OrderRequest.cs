namespace SaralApi.DTOs.Orders
{
    public class OrderRequest
    {
        // The list of products and how many the user wants
        public List<OrderItemRequest> Items { get; set; } = new();

        // Optional: Payment details for simulation
        public string CreditCardNumber { get; set; } = string.Empty;
    }
}
