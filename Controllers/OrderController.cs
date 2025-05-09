using FoodDelivery.Domain.Contracts;
using FoodDelivery.DTO.OrderDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodDelivery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetClientId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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
        public async Task<IActionResult> MarkAsPaid(int orderId)
        {
            return await _orderService.OrderPaidAsync(orderId)
                ? Ok() : BadRequest();
        }

        [HttpPost("{orderId}/cancel")]
        public async Task<IActionResult> Cancel(int orderId)
        {
            return await _orderService.CancelOrderAsync(orderId)
                ? Ok() : BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetClientOrders()
        {
            var clientId = GetClientId();
            var orders = await _orderService.GetClientOrdersAsync(clientId);
            return Ok(orders);
        }

        [Authorize]
        [HttpPost("{orderId}/deliver")]
        public async Task<IActionResult> Deliver(int orderId)
        {
            return await _orderService.OrderDeliveredAsync(orderId)
                ? Ok() : BadRequest();
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var clientId = GetClientId();
            var order = await _orderService.GetOrderDetailsAsync(orderId);

            if (order == null || (await _orderService.GetClientOrdersAsync(clientId))
                .All(x => x.OrderId != orderId))
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}
