using FoodDelivery.Domain.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDelivery.DTO.CartDTO;

namespace FoodDelivery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Client")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetClientId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var clientId = GetClientId();
            var cart = await _cartService.GetCartAsync(clientId);
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int dishId)
        {
            var clientId = GetClientId();
            await _cartService.AddToCartAsync(clientId, dishId);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateQuantity([FromBody] CartItemDTO dto)
        {
            var clientId = GetClientId();
            await _cartService.UpdateQuantityAsync(clientId, dto.DishId, dto.Quantity);
            return Ok();
        }

        [HttpDelete("items/{dishId}")]
        public async Task<IActionResult> RemoveFromCart(int dishId)
        {
            var clientId = GetClientId();
            await _cartService.RemoveFromCartAsync(clientId, dishId);
            return Ok();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var clientId = GetClientId();
            await _cartService.ClearCartAsync(clientId);
            return Ok();
        }
    }
}
