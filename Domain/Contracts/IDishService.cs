using FoodDelivery.Domain.Models;

namespace FoodDelivery.Domain.Contracts
{
    public interface IDishService
    {
        Task<IEnumerable<Dish>> GetAllAsync();
    }
}
