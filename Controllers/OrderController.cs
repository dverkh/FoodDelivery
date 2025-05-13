using FoodDelivery.Domain.Contracts;
using FoodDelivery.DTO.OrderDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodDelivery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetClientId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [Authorize(Roles = "Client")]
        [HttpGet()]
        public async Task<IActionResult> GetClientOrders()
        {
            var clientId = GetClientId();
            var orders = await _orderService.GetClientOrdersAsync(clientId);
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("paid")]
        public async Task<IActionResult> GetPaidOrders() => Ok(await _orderService.GetPaidOrdersAsync());

        [Authorize(Roles = "Client")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] string deliveryAddress)
        {
            var clientId = GetClientId();
            var orderCreate = new OrderCreateDTO
            {
                ClientId = clientId,
                DeliveryAddress = deliveryAddress,
            };
            var order = await _orderService.CreateOrderAsync(orderCreate);
            return Ok(order);
        }


        [HttpPost("{orderId}/pay")]
        public async Task<IActionResult> MarkOrderAsPaid(int orderId)
        {
            return await _orderService.OrderPayAsync(orderId)
                ? Ok() : BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            return await _orderService.CancelOrderAsync(orderId)
                ? Ok() : BadRequest();
        }

        [Authorize]
        [HttpPost("{orderId}/deliver")]
        public async Task<IActionResult> MarkOrdeAsrDelivered(int orderId)
        {
            return await _orderService.OrderDeliveredAsync(orderId)
                ? Ok() : BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{orderId}/courier")]
        public async Task<IActionResult> OrderGivenToCourier(int orderId)
        {
            return await _orderService.OrderGivenToCourierAsync(orderId)
                ? Ok() : BadRequest();
        }
    }
}
