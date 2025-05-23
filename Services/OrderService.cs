using FoodDelivery.Domain.Contracts;
using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.OrderDTO;
using FoodDelivery.Storage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services
{
    /// <summary>
    /// Реализация сервиса для управления заказами
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly FoodDeliveryContext _context;
        private readonly ICartService _cartService;

        public OrderService(FoodDeliveryContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        /// <summary>
        /// Получает заказ с указанным идентификатором
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Заказ с указанным идентификатором</returns>
        public async Task<OrderResponseDTO?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null) return null;

            return new OrderResponseDTO
            {
                OrderId = order.OrderId,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                DateTime = order.DateTime,
                DeliveryAddress = order.DeliveryAddress,
                CourierId = order.CourierId,
                Items = order.OrderDetails.Select(od => new OrderItemDTO
                {
                    DishId = od.DishId,
                    DishName = od.Dish.Name,
                    Quantity = od.Quantity,
                    Price = od.Dish.Price
                }).ToList()
            };
        }

        /// <summary>
        /// Создает новый заказ на основе содержимого корзины клиента
        /// </summary>
        /// <param name="dto">Данные для создания заказа</param>
        /// <returns>Информация о созданном заказе</returns>
        /// <exception cref="InvalidOperationException">Возникает, если корзина пуста</exception>
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
                CourierId = null,
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
                CourierId = order.CourierId,
                Items = order.OrderDetails.Select(x => new OrderItemDTO
                {
                    DishId = x.DishId,
                    DishName = x.Dish.Name,
                    Quantity = x.Quantity,
                    Price = x.Dish.Price
                }).ToList()
            };
        }

        /// <summary>
        /// Отмечает заказ как оплаченный
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Обновленный заказ</returns>
        public async Task<OrderResponseDTO?> OrderPayAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Создан")
                return null;

            order.Status = "Оплачен";
            await _context.SaveChangesAsync();
            return await GetOrderByIdAsync(orderId);
        }


        /// <summary>
        /// Отменяет заказ
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Обновленный заказ</returns>
        public async Task<OrderResponseDTO?> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status == "Доставлен")
                return null;

            order.Status = "Отменен";
            await _context.SaveChangesAsync();
            return await GetOrderByIdAsync(orderId);
        }

        /// <summary>
        /// Отмечает заказ как доставленный
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Обновленный заказ</returns>
        public async Task<OrderResponseDTO?> OrderDeliveredAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Оплачен")
                return null;

            order.Status = "Доставлен";
            await _context.SaveChangesAsync();
            return await GetOrderByIdAsync(orderId);
        }

        /// <summary>
        /// Получает список всех заказов клиента
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <returns>Список заказов клиента</returns>
        public async Task<List<OrderResponseDTO>> GetClientOrdersAsync(int clientId)
        {
            var orders = await _context.Orders
                .Where(o => o.ClientId == clientId)
                .OrderByDescending(o => o.DateTime)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .ToListAsync();

            return orders.Select(order => new OrderResponseDTO
            {
                OrderId = order.OrderId,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                DateTime = order.DateTime,
                DeliveryAddress = order.DeliveryAddress,
                CourierId = order.CourierId,
                Items = order.OrderDetails.Select(od => new OrderItemDTO
                {
                    DishId = od.DishId,
                    DishName = od.Dish.Name,
                    Quantity = od.Quantity,
                    Price = od.Dish.Price
                }).ToList()
            }).ToList();
        }

        /// <summary>
        /// Получает список всех оплаченных заказов
        /// </summary>
        /// <returns>Коллекция оплаченных заказов</returns>
        public async Task<IEnumerable<OrderResponseDTO>> GetPaidOrdersAsync()
        {
            var orders = await _context.Orders
                .Where(o => o.Status == "Оплачен")
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Dish)
                .ToListAsync();

            return orders.Select(o => new OrderResponseDTO
            {
                OrderId = o.OrderId,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                DateTime = o.DateTime,
                DeliveryAddress = o.DeliveryAddress,
                CourierId = o.CourierId,
                Items = o.OrderDetails.Select(od => new OrderItemDTO
                {
                    DishId = od.DishId,
                    DishName = od.Dish.Name,
                    Quantity = od.Quantity
                }).ToList()
            });
        }

        /// <summary>
        /// Отмечает заказ как переданный курьеру
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// /// <param name="courierId">Идентификатор курьера</param>
        /// <returns>Обновленный заказ</returns>
        public async Task<OrderResponseDTO?> OrderGivenToCourierAsync(int orderId, int courierId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Оплачен")
                return null;

            var courierExists = await _context.Couriers.AnyAsync(c => c.CourierId == courierId);
            if (!courierExists)
                return null;

            order.Status = "Передан курьеру";
            order.CourierId = courierId;
            await _context.SaveChangesAsync();
            return await GetOrderByIdAsync(orderId);
        }
    }
}
