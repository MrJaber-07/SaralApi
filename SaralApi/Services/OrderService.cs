using Microsoft.EntityFrameworkCore;
using SaralApi.DTOs.Orders;
using SaralApi.Models;
using SaralApi.Repositories;

namespace SaralApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly AppDbContext _context;

        public OrderService(
            IRepository<Order> orderRepo,
            IRepository<Product> productRepo,
            AppDbContext context)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _context = context;
        }

        // --- CREATE: Transactional Order Logic ---
        public async Task<string> PlaceOrderAsync(OrderRequest request, string idempotencyKey)
        {
            // 1. Idempotency Check: Prevent duplicate orders
            var allOrders = await _orderRepo.GetAllAsync();
            var existingOrder = allOrders.FirstOrDefault(o => o.IdempotencyKey == idempotencyKey);

            if (existingOrder != null)
                return $"Duplicate Request: Order {existingOrder.Id} already exists.";

            // 2. Atomic Transaction: Ensures all inventory changes or none are saved
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    IdempotencyKey = idempotencyKey,
                    Status = OrderStatus.PROCESSING,
                    CreatedAt = DateTime.UtcNow,
                    Items = new List<OrderItem>()
                };

                await _orderRepo.AddAsync(order);

                // 3. Handle Inventory & Partial Failures
                foreach (var item in request.Items)
                {
                    var product = await _productRepo.GetByIdAsync(item.ProductId);

                    if (product == null || product.Stock < item.Quantity)
                    {
                        throw new InvalidOperationException($"Product {item.ProductId} out of stock.");
                    }

                    // Deduct stock
                    product.Stock -= item.Quantity;
                    _productRepo.Update(product);

                    order.Items.Add(new OrderItem { ProductId = item.ProductId, Quantity = item.Quantity });
                }

                // Initial save to check for Concurrency (RowVersion)
                await _context.SaveChangesAsync();

                // 4. Payment Simulation
                bool isPaymentSuccessful = SimulatePayment();

                if (isPaymentSuccessful)
                {
                    order.Status = OrderStatus.SUCCESS;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return $"Order {order.Id} Confirmed Successfully.";
                }
                else
                {
                    throw new Exception("Payment Gateway Failure.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return "Concurrency Error: Stock was updated by another user. Please retry.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Order Failed: {ex.Message}. Inventory has been rolled back.";
            }
        }

        // --- READ: Get All Orders ---
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepo.GetAllAsync();
        }

        // --- READ: Get Single Order ---
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _orderRepo.GetByIdAsync(id);
        }

        // --- UPDATE: Modify Order ---
        public async Task<bool> UpdateOrderAsync(Order order)
        {
            try
            {
                _orderRepo.Update(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // --- DELETE: Remove Order ---
        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return false;

            _orderRepo.Delete(order);
            await _context.SaveChangesAsync();
            return true;
        }

        private bool SimulatePayment()
        {
            return new Random().Next(0, 10) > 2; // 80% success rate
        }
    }
}