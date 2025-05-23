using FoodDelivery.Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDelivery.DTO.CartDTO;

namespace FoodDelivery.Controllers
{
    /// <summary>
    /// Контроллер для управления корзиной покупок клиента
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Client")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера корзины
        /// </summary>
        /// <param name="cartService">Сервис корзины</param>
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Получает идентификатор текущего клиента из токена
        /// </summary>
        /// <returns>Идентификатор клиента</returns>
        private int GetClientId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Получает содержимое корзины текущего клиента
        /// </summary>
        /// <returns>Список товаров в корзине</returns>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var clientId = GetClientId();
            var cart = await _cartService.GetCartAsync(clientId);
            return Ok(cart);
        }

        /// <summary>
        /// Добавляет блюдо в корзину
        /// </summary>
        /// <param name="dishId">Идентификатор блюда</param>
        /// <returns>Результат операции добавления</returns>
        [HttpPost("{dishId}")]
        public async Task<IActionResult> AddToCart(int dishId)
        {
            var clientId = GetClientId();
            var cart = await _cartService.AddToCartAsync(clientId, dishId);
            return Ok(cart);
        }

        /// <summary>
        /// Обновляет количество блюда в корзине
        /// </summary>
        /// <param name="dto">Данные для обновления количества</param>
        /// <returns>Результат операции обновления</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateQuantity([FromBody] CartItemDTO dto)
        {
            var clientId = GetClientId();
            var cart = await _cartService.UpdateQuantityAsync(clientId, dto.DishId, dto.Quantity);
            return Ok(cart);
        }

        /// <summary>
        /// Удаляет блюдо из корзины
        /// </summary>
        /// <param name="dishId">Идентификатор блюда</param>
        /// <returns>Результат операции удаления</returns>
        [HttpDelete("{dishId}")]
        public async Task<IActionResult> RemoveFromCart(int dishId)
        {
            var clientId = GetClientId();
            var cart = await _cartService.RemoveFromCartAsync(clientId, dishId);
            return Ok(cart);
        }

        /// <summary>
        /// Очищает корзину текущего клиента
        /// </summary>
        /// <returns>Результат операции очистки</returns>
        [HttpDelete("all")]
        public async Task<IActionResult> ClearCart()
        {
            var clientId = GetClientId();
            var cart = await _cartService.ClearCartAsync(clientId);
            return Ok(cart);
        }
    }
}
