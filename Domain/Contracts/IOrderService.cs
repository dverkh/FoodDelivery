using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.OrderDTO;

namespace FoodDelivery.Domain.Contracts
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> CreateOrderAsync(OrderCreateDTO dto);
        Task<bool> OrderPayAsync(int orderId);
        Task<bool> CancelOrderAsync(int orderId);
        Task<bool> OrderDeliveredAsync(int orderId);
        Task<List<OrderResponseDTO>> GetClientOrdersAsync(int clientId);
        Task<IEnumerable<OrderResponseDTO>> GetPaidOrdersAsync();
        Task<bool> OrderGivenToCourierAsync(int orderId);
    }
}
