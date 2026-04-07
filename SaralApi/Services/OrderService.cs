using Microsoft.EntityFrameworkCore;
using SaralApi.DTOs.Orders;
using SaralApi.Interface;
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

        public async Task<string> PlaceOrderAsync(OrderRequest request, string idempotencyKey)
        {
            // 1. Idempotency Check
            var allOrders = await _orderRepo.GetAllAsync();
            var existingOrder = allOrders.FirstOrDefault(o => o.IdempotencyKey == idempotencyKey);

            if (existingOrder != null)
                return $"Duplicate Request: Order {existingOrder.Id} already exists.";

            // 2. Atomic Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    IdempotencyKey = idempotencyKey,
                    Status = OrderStatus.PROCESSING,
                    CreatedAt = DateTime.UtcNow
                };

                await _orderRepo.AddAsync(order);

                // 3. Handle Concurrency & Partial Failures
                foreach (var item in request.Items)
                {
                    var product = await _productRepo.GetByIdAsync(item.ProductId);

                    if (product == null || product.Stock < item.Quantity)
                    {
                        throw new InvalidOperationException($"Product {item.ProductId} out of stock.");
                    }

                    product.Stock -= item.Quantity;
                    _productRepo.Update(product);

                    order.Items.Add(new OrderItem { ProductId = item.ProductId, Quantity = item.Quantity });
                }

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

        private bool SimulatePayment()
        {
            return new Random().Next(0, 10) > 2; // 80% success
        }
    }
}