using FoodDelivery.DTO;

namespace FoodDelivery.Domain.Contracts
{
    /// <summary>
    /// Сервис для управления курьерами
    /// </summary>
    public interface ICourierService
    {
        /// <summary>
        /// Сервис для добавления курьера
        /// </summary>
        Task AddCourierAsync(CreateCourierDTO dto);
    }
}
