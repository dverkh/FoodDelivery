using Microsoft.AspNetCore.Mvc;
using FoodDelivery.Domain.Models;
using FoodDelivery.Domain.Contracts;
using FoodDelivery.DTO.DishDTO;
using Microsoft.AspNetCore.Authorization;

namespace FoodDelivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dish>>> GetDishes()
        {
            var dishes = await _dishService.GetAllAsync();
            return Ok(dishes);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("dish")]
        public async Task<IActionResult> AddDish([FromBody] DishDTO dishDto)
        {
            await _dishService.AddDishAsync(dishDto);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("category")]
        public async Task<IActionResult> AddCategory([FromBody] string name)
        {
            await _dishService.AddCategoryAsync(name);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("dish/{id}")]
        public async Task<IActionResult> UpdateDish(int id, [FromBody] DishDTO dishDto)
        {
            try
            {
                await _dishService.UpdateDishAsync(id, dishDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("category/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, string newName)
        {
            try
            {
                await _dishService.UpdateCategoryAsync(id, newName);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
