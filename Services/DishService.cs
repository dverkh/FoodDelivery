using FoodDelivery.Domain.Contracts;
using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.DishDTO;
using FoodDelivery.Storage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services
{
    public class DishService : IDishService
    {
        private readonly FoodDeliveryContext _context;

        public DishService(FoodDeliveryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dish>> GetAllAsync()
        {
            return await _context.Dishes.ToListAsync();
        }

        public async Task AddDishAsync(DishDTO dishDto)
        {
            var dish = new Dish
            {
                Name = dishDto.Name,
                CategoryId = dishDto.CategoryId,
                Description = dishDto.Description,
                Price = dishDto.Price,
                IsAvailable = dishDto.IsAvailable
            };

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDishAsync(int id, DishDTO dishDto)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null)
                throw new Exception();

            dish.Name = dishDto.Name;
            dish.CategoryId = dishDto.CategoryId;
            dish.Description = dishDto.Description;
            dish.Price = dishDto.Price;
            dish.IsAvailable = dishDto.IsAvailable;

            await _context.SaveChangesAsync();
        }

        public async Task AddCategoryAsync(string newName)
        {
            var category = new DishCategory
            {
                Name = newName,
            };

        _context.DishCategories.Add(category);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCategoryAsync(int id, string newName)
        {
            var category = await _context.DishCategories.FindAsync(id);
            if (category == null)
                throw new Exception();

            category.Name = newName;

            await _context.SaveChangesAsync();
        }
    }
}
