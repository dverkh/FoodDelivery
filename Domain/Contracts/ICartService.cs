using FoodDelivery.DTO.CartDTO;

namespace FoodDelivery.Domain.Contracts
{
    public interface ICartService
    {
        Task<List<CartResponseDTO>> GetCartAsync(int clientId);
        Task AddToCartAsync(int clientId, int dishId);
        Task UpdateQuantityAsync(int clientId, int dishId, int quantity);
        Task RemoveFromCartAsync(int clientId, int dishId);
        Task ClearCartAsync(int clientId);
    }
}
