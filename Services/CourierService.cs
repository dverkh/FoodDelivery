using FoodDelivery.Domain.Models;
using FoodDelivery.DTO;
using FoodDelivery.Storage.EntityConfigurations;
using FoodDelivery.Domain.Contracts;

namespace FoodDelivery.Services
{
    /// <summary>
    /// Контроллер для управления курьерами.
    /// </summary>
    public class CourierService : ICourierService
    {
        private readonly FoodDeliveryContext _context;

        public CourierService(FoodDeliveryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Сервис для добавления курьера.
        /// </summary>
        /// <param name="dto">Данные курьера</param>
        public async Task AddCourierAsync(CreateCourierDTO dto)
        {
            var courier = new Courier
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber
            };

            _context.Couriers.Add(courier);
            await _context.SaveChangesAsync();
        }
    }
}
