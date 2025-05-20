using FoodDelivery.DTO.CartDTO;

namespace FoodDelivery.Domain.Contracts
{
    /// <summary>
    /// Сервис для управления корзиной покупок клиента
    /// </summary>
    public interface ICartService
    {
        /// <summary>
        /// Получает содержимое корзины клиента
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <returns>Список товаров в корзине</returns>
        Task<List<CartResponseDTO>> GetCartAsync(int clientId);

        /// <summary>
        /// Добавляет блюдо в корзину клиента
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="dishId">Идентификатор блюда</param>
        Task AddToCartAsync(int clientId, int dishId);

        /// <summary>
        /// Обновляет количество блюда в корзине
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="dishId">Идентификатор блюда</param>
        /// <param name="quantity">Новое количество</param>
        Task UpdateQuantityAsync(int clientId, int dishId, int quantity);

        /// <summary>
        /// Удаляет блюдо из корзины
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="dishId">Идентификатор блюда</param>
        Task RemoveFromCartAsync(int clientId, int dishId);

        /// <summary>
        /// Очищает корзину клиента
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        Task ClearCartAsync(int clientId);
    }
}
