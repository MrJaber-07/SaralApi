using SaralApi.DTOs.Orders;
using SaralApi.Models;

namespace SaralApi.Services
{
    public interface IOrderService
    {
        Task<string> PlaceOrderAsync(OrderRequest request, string idempotencyKey);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int id);
    }
}
