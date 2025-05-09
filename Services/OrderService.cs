using FoodDelivery.Domain.Contracts;
using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.OrderDTO;
using FoodDelivery.Storage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services
{
    public class OrderService : IOrderService
    {
        private readonly FoodDeliveryContext _context;
        private readonly ICartService _cartService;

        public OrderService(FoodDeliveryContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<OrderResponseDTO> CreateOrderAsync(OrderCreateDTO dto)
        {
            var cartItems = await _context.CartItems
                .Where(x => x.ClientId == dto.ClientId)
                .Include(x => x.Dish)
                .ToListAsync();

            if (!cartItems.Any())
            {
                throw new InvalidOperationException("Корзина пуста");
            }

            var totalPrice = cartItems.Sum(x => x.Dish.Price * x.Quantity);

            var order = new Order
            {
                ClientId = dto.ClientId,
                Status = "Создан",
                TotalPrice = totalPrice,
                DateTime = DateTime.UtcNow,
                DeliveryAddress = dto.DeliveryAddress,
                OrderDetails = cartItems.Select(x => new OrderDetails
                {
                    DishId = x.DishId,
                    Quantity = x.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);

            await _cartService.ClearCartAsync(dto.ClientId);

            await _context.SaveChangesAsync();

            return new OrderResponseDTO
            {
                OrderId = order.OrderId,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                DateTime = order.DateTime,
                DeliveryAddress = order.DeliveryAddress,
                Items = order.OrderDetails.Select(x => new OrderItemDTO
                {
                    DishId = x.DishId,
                    DishName = x.Dish.Name,
                    Quantity = x.Quantity,
                    Price = x.Dish.Price
                }).ToList()
            };
        }

        public async Task<bool> OrderPaidAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Создан")
                return false;

            order.Status = "Оплачен";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Создан")
                return false;

            order.Status = "Отменен";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> OrderDeliveredAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Оплачен")
                return false;

            order.Status = "Доставлен";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<OrderListDTO>> GetClientOrdersAsync(int clientId)
        {
            return await _context.Orders
                .Where(x => x.ClientId == clientId)
                .OrderByDescending(x => x.DateTime)
                .Select(x => new OrderListDTO
                {
                    OrderId = x.OrderId,
                    Status = x.Status,
                    TotalPrice = x.TotalPrice,
                    DateTime = x.DateTime,
                    DeliveryAddress = x.DeliveryAddress
                })
                .ToListAsync();
        }

        public async Task<OrderResponseDTO> GetOrderDetailsAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Dish)
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException("Заказ не найден");
            }

            return new OrderResponseDTO
            {
                OrderId = order.OrderId,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                DateTime = order.DateTime,
                DeliveryAddress = order.DeliveryAddress,
                Items = order.OrderDetails.Select(x => new OrderItemDTO
                {
                    DishId = x.DishId,
                    DishName = x.Dish.Name,
                    Quantity = x.Quantity,
                    Price = x.Dish.Price
                }).ToList()
            };
        }
    }
}
