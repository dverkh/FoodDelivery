using FoodDelivery.Domain.Contracts;
using FoodDelivery.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controllers
{
    /// <summary>
    /// Контроллер для управления курьерами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CourierController : ControllerBase
    {
        private readonly ICourierService _courierService;

        public CourierController(ICourierService courierService)
        {
            _courierService = courierService;
        }

        /// <summary>
        /// Метод для добавление нового курьера
        /// </summary>
        /// <param name="dto">Данные курьера</param>
        /// <returns>Результат операции добавления</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCourier([FromBody] CreateCourierDTO dto)
        {
            await _courierService.AddCourierAsync(dto);
            return Ok("Курьер успешно добавлен.");
        }
    }
}
