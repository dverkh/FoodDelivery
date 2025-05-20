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

        /// <summary>
        /// Отмечает заказ как оплаченный
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>true если статус успешно обновлен, иначе false</returns>
        public async Task<bool> OrderPayAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Создан")
                return false;

            order.Status = "Оплачен";
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Отменяет заказ
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>true если заказ успешно отменен, иначе false</returns>
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status == "Доставлен")
                return false;

            order.Status = "Отменен";
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Отмечает заказ как доставленный
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>true если статус успешно обновлен, иначе false</returns>
        public async Task<bool> OrderDeliveredAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Оплачен")
                return false;

            order.Status = "Доставлен";
            await _context.SaveChangesAsync();
            return true;
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
        /// <returns>true если статус успешно обновлен, иначе false</returns>
        public async Task<bool> OrderGivenToCourierAsync(int orderId, int courierId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Оплачен")
                return false;

            var courierExists = await _context.Couriers.AnyAsync(c => c.CourierId == courierId);
            if (!courierExists)
                return false;

            order.Status = "Передан курьеру";
            order.CourierId = courierId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
