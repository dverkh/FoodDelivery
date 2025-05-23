using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.OrderDTO;

namespace FoodDelivery.Domain.Contracts
{
    /// <summary>
    /// Сервис для управления заказами
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Получает заказ с указанным идентификатором
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Заказ с указанным идентификатором</returns>
        Task<OrderResponseDTO?> GetOrderByIdAsync(int orderId);

        /// <summary>
        /// Создает новый заказ на основе содержимого корзины клиента
        /// </summary>
        /// <param name="dto">Данные для создания заказа</param>
        /// <returns>Информация о созданном заказе</returns>
        Task<OrderResponseDTO> CreateOrderAsync(OrderCreateDTO dto);

        /// <summary>
        /// Отмечает заказ как оплаченный
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Обновленный заказ</returns>
        Task<OrderResponseDTO?> OrderPayAsync(int orderId);

        /// <summary>
        /// Отменяет заказ
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Обновленный заказ</returns>
        Task<OrderResponseDTO?> CancelOrderAsync(int orderId);

        /// <summary>
        /// Отмечает заказ как доставленный
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Обновленный заказ</returns>
        Task<OrderResponseDTO?> OrderDeliveredAsync(int orderId);

        /// <summary>
        /// Получает список всех заказов клиента
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <returns>Список заказов клиента</returns>
        Task<List<OrderResponseDTO>> GetClientOrdersAsync(int clientId);

        /// <summary>
        /// Получает список всех оплаченных заказов
        /// </summary>
        /// <returns>Коллекция оплаченных заказов</returns>
        Task<IEnumerable<OrderResponseDTO>> GetPaidOrdersAsync();

        /// <summary>
        /// Отмечает заказ как переданный курьеру
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Обновленный заказ</returns>
        Task<OrderResponseDTO?> OrderGivenToCourierAsync(int orderId, int courierId);
    }
}
