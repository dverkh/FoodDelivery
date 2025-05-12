using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.DishDTO;

namespace FoodDelivery.Domain.Contracts
{
    public interface IDishService
    {
        Task<IEnumerable<Dish>> GetAllAsync();
        Task AddDishAsync(DishDTO dish);
        Task UpdateDishAsync(int id, DishDTO dish);
        Task AddCategoryAsync(string name);
        Task UpdateCategoryAsync(int id, string name);
    }
}
