using Microsoft.AspNetCore.Mvc;
using FoodDelivery.Domain.Models;
using FoodDelivery.Domain.Contracts;
using FoodDelivery.DTO.DishDTO;
using Microsoft.AspNetCore.Authorization;

namespace FoodDelivery.Controllers
{
    /// <summary>
    /// Контроллер для управления блюдами и их категориями
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера блюд
        /// </summary>
        /// <param name="dishService">Сервис управления блюдами</param>
        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        /// <summary>
        /// Получает список всех блюд
        /// </summary>
        /// <returns>Коллекция всех доступных блюд</returns>
        [HttpGet]
        public async Task<IActionResult> GetDishes()
        {
            var menu = await _dishService.GetAllAsync();
            return Ok(menu);
        }

        /// <summary>
        /// Добавляет новое блюдо в меню
        /// </summary>
        /// <param name="dishDto">Данные нового блюда</param>
        /// <returns>Результат операции добавления</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("dish")]
        public async Task<IActionResult> AddDish([FromBody] DishDTO dishDto)
        {
            var updatedMenu = await _dishService.AddDishAsync(dishDto);
            return Ok(updatedMenu);
        }

        /// <summary>
        /// Создает новую категорию блюд
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <returns>Результат операции создания</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("category")]
        public async Task<IActionResult> AddCategory([FromBody] string name)
        {
            await _dishService.AddCategoryAsync(name);
            return Ok("Новая категория блюд создана");
        }

        /// <summary>
        /// Обновляет информацию о блюде
        /// </summary>
        /// <param name="dishId">Идентификатор блюда</param>
        /// <param name="dishDto">Новые данные блюда</param>
        /// <returns>Результат операции обновления</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{dishId}")]
        public async Task<IActionResult> UpdateDish(int dishId, [FromBody] DishDTO dishDto)
        {
            try
            {
                var updatedMenu = await _dishService.UpdateDishAsync(dishId, dishDto);
                return Ok(updatedMenu);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Блюдо не найдено");
            }
        }

        /// <summary>
        /// Обновляет название категории блюд
        /// </summary>
        /// <param name="dishId">Идентификатор категории</param>
        /// <param name="newName">Новое название категории</param>
        /// <returns>Результат операции обновления</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("category/{dishId}")]
        public async Task<IActionResult> UpdateCategory(int dishId, string newName)
        {
            try
            {
                await _dishService.UpdateCategoryAsync(dishId, newName);
                return Ok("Категория обновлена");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
