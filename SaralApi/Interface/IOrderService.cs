using SaralApi.DTOs.Orders;

namespace SaralApi.Interface
{
    public interface IOrderService
    {
        Task<string> PlaceOrderAsync(OrderRequest request, string idempotencyKey);
    }
}
