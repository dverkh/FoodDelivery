using FoodDelivery.Domain.Contracts;
using FoodDelivery.Domain.Models;
using FoodDelivery.Storage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using FoodDelivery.DTO.CartDTO;


namespace FoodDelivery.Services
{
    public class CartService : ICartService
    {
        private readonly FoodDeliveryContext _context;

        public CartService(FoodDeliveryContext context)
        {
            _context = context;
        }

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

        public async Task AddToCartAsync(int clientId, int dishId)
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
        }

        public async Task UpdateQuantityAsync(int clientId, int dishId, int quantity)
        {
            var item = await _context.CartItems.FindAsync(clientId, dishId);
            if (item != null)
            {
                item.Quantity = quantity;
                if (item.Quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(int clientId, int dishId)
        {
            var item = await _context.CartItems.FindAsync(clientId, dishId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int clientId)
        {
            var items = _context.CartItems.Where(x => x.ClientId == clientId);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
