using Microsoft.AspNetCore.Mvc;
using FoodDelivery.Domain.Models;
using FoodDelivery.Domain.Contracts;

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
    }
}
