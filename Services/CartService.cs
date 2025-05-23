using FoodDelivery.Domain.Contracts;
using FoodDelivery.Domain.Models;
using FoodDelivery.Storage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using FoodDelivery.DTO.CartDTO;

namespace FoodDelivery.Services
{
    /// <summary>
    /// Реализация сервиса для управления корзиной покупок клиента
    /// </summary>
    public class CartService : ICartService
    {
        private readonly FoodDeliveryContext _context;

        public CartService(FoodDeliveryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает содержимое корзины клиента
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <returns>Список товаров в корзине</returns>
        public async Task<List<CartResponseDTO>> GetCartAsync(int clientId)
        {
            return await _context.CartItems
                .Where(x => x.ClientId == clientId)
                .Include(x => x.Dish)
                .Select(x => new CartResponseDTO
                {
                    DishId = x.DishId,
                    DishName = x.Dish.Name,
                    DishPrice = x.Dish.Price,
                    Quantity = x.Quantity
                })
                .ToListAsync();
        }

        /// <summary>
        /// Добавляет блюдо в корзину клиента
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="dishId">Идентификатор блюда</param>
        public async Task<List<CartResponseDTO>> AddToCartAsync(int clientId, int dishId)
        {
            var existingItem = await _context.CartItems.FindAsync(clientId, dishId);

            if (existingItem != null)
            {
                existingItem.Quantity = 1;
            }
            else
            {
                var newItem = new CartItem
                {
                    ClientId = clientId,
                    DishId = dishId,
                    Quantity = 1
                };

                await _context.CartItems.AddAsync(newItem);
            }

            await _context.SaveChangesAsync();
            return await GetCartAsync(clientId);
        }

        /// <summary>
        /// Обновляет количество блюда в корзине
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="dishId">Идентификатор блюда</param>
        /// <param name="quantity">Новое количество</param>
        public async Task<List<CartResponseDTO>> UpdateQuantityAsync(int clientId, int dishId, int quantity)
        {
            var item = await _context.CartItems.FindAsync(clientId, dishId);
            if (item == null)
            {
                throw new KeyNotFoundException("Блюдо в корзине не найдено");
            }
            item.Quantity = quantity;
            if (item.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }

            await _context.SaveChangesAsync();
            return await GetCartAsync(clientId);
        }

        /// <summary>
        /// Удаляет блюдо из корзины
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="dishId">Идентификатор блюда</param>
        public async Task<List<CartResponseDTO>> RemoveFromCartAsync(int clientId, int dishId)
        {
            var item = await _context.CartItems.FindAsync(clientId, dishId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
            }

            await _context.SaveChangesAsync();
            return await GetCartAsync(clientId);
        }

        /// <summary>
        /// Очищает корзину клиента
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        public async Task<List<CartResponseDTO>> ClearCartAsync(int clientId)
        {
            var items = _context.CartItems.Where(x => x.ClientId == clientId);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
            return await GetCartAsync(clientId);
        }
    }
}
