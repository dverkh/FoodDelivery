using FoodDelivery.DTO.OrderDTO;

namespace FoodDelivery.Domain.Contracts
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> CreateOrderAsync(OrderCreateDTO dto);
        Task<bool> OrderPaidAsync(int orderId);
        Task<bool> CancelOrderAsync(int orderId);
        Task<bool> OrderDeliveredAsync(int orderId);
        Task<List<OrderListDTO>> GetClientOrdersAsync(int clientId);
        Task<OrderResponseDTO> GetOrderDetailsAsync(int orderId);
    }
}
