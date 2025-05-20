using FoodDelivery.Domain.Contracts;
using FoodDelivery.DTO.OrderDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodDelivery.Controllers
{
    /// <summary>
    /// Контроллер для управления заказами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера заказов
        /// </summary>
        /// <param name="orderService">Сервис управления заказами</param>
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Получает идентификатор текущего клиента из токена
        /// </summary>
        /// <returns>Идентификатор клиента</returns>
        private int GetClientId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Получает список заказов текущего клиента
        /// </summary>
        /// <returns>Список заказов клиента</returns>
        [Authorize(Roles = "Client")]
        [HttpGet()]
        public async Task<IActionResult> GetClientOrders()
        {
            var clientId = GetClientId();
            var orders = await _orderService.GetClientOrdersAsync(clientId);
            return Ok(orders);
        }

        /// <summary>
        /// Получает список всех оплаченных заказов
        /// </summary>
        /// <returns>Список оплаченных заказов</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("paid")]
        public async Task<IActionResult> GetPaidOrders() => Ok(await _orderService.GetPaidOrdersAsync());

        /// <summary>
        /// Создает новый заказ
        /// </summary>
        /// <param name="deliveryAddress">Адрес доставки</param>
        /// <returns>Информация о созданном заказе</returns>
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

        /// <summary>
        /// Отмечает заказ как оплаченный
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Результат операции оплаты</returns>
        [Authorize(Roles = "Client")]
        [HttpPut("{orderId}/pay")]
        public async Task<IActionResult> MarkOrderAsPaid(int orderId)
        {
            return await _orderService.OrderPayAsync(orderId)
                ? Ok("Заказ успешно оплачен") : BadRequest("Ошибка оплаты");
        }

        /// <summary>
        /// Отменяет заказ
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Результат операции отмены</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            return await _orderService.CancelOrderAsync(orderId)
                ? Ok("Заказ отменен") : BadRequest();
        }

        /// <summary>
        /// Отмечает заказ как переданный курьеру
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Результат операции передачи курьеру</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}/courier")]
        public async Task<IActionResult> OrderGivenToCourier(int orderId)
        {
            return await _orderService.OrderGivenToCourierAsync(orderId)
                ? Ok("Заказ отмечен как переданный курьеру") : BadRequest();
        }

        /// <summary>
        /// Отмечает заказ как доставленный
        /// </summary>
        /// <param name="orderId">Идентификатор заказа</param>
        /// <returns>Результат операции доставки</returns>
        [Authorize(Roles = "Admin,Client")]
        [HttpPut("{orderId}/deliver")]
        public async Task<IActionResult> MarkOrdeAsrDelivered(int orderId)
        {
            return await _orderService.OrderDeliveredAsync(orderId)
                ? Ok("Заказ отмечен как доставленный") : BadRequest();
        }
    }
}
