using System.Security.Claims;
using FoodDelivery.Domain.Contracts;
using FoodDelivery.DTO.ClientDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controllers
{
    /// <summary>
    /// Контроллер для управления данными и операциями клиента
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Client")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Получает идентификатор клиента из токена доступа
        /// </summary>
        /// <returns>Идентификатор авторизованного клиента</returns>
        private int GetClientId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Получает информацию профиля клиента
        /// </summary>
        /// <returns>Ответ OK с данными клиента</returns>
        [HttpGet]
        public async Task<IActionResult> GetClient()
        {
            var clientId = GetClientId();
            var client = await _clientService.GetClientAsync(clientId);
            return Ok(client);
        }

        /// <summary>
        /// Обновляет отображаемое имя клиента
        /// </summary>
        /// <param name="newName">Новое отображаемое имя клиента</param>
        /// <returns>Ответ OK при успешном обновлении имени</returns>
        [HttpPut("name")]
        public async Task<IActionResult> UpdateName([FromBody] string newName)
        {
            var clientId = GetClientId();
            await _clientService.UpdateNameAsync(clientId, newName);
            return Ok("Имя изменено");
        }

        /// <summary>
        /// Обновляет номер телефона клиента
        /// </summary>
        /// <param name="newPhone">Новый номер телефона клиента</param>
        /// <returns>Ответ OK при успешном обновлении номера телефона</returns>
        [HttpPut("phone")]
        public async Task<IActionResult> UpdatePhone([FromBody] string newPhone)
        {
            var clientId = GetClientId();
            await _clientService.UpdatePhoneAsync(clientId, newPhone);
            return Ok("Телефон изменен");
        }

        /// <summary>
        /// Обновляет пароль клиента после проверки старого пароля
        /// </summary>
        /// <param name="dto">DTO, содержащий старый и новый пароли</param>
        /// <returns>Ответ OK при успешной смене пароля или BadRequest с соответствующим сообщением об ошибке</returns>
        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordUpdateDTO dto)
        {
            var clientId = GetClientId();
            var passwordUpdate = new PasswordUpdateDTO
            {
                oldPassword = dto.oldPassword,
                newPassword = dto.newPassword
            };
            string PasswordUpdateResult = await _clientService.UpdatePasswordAsync(clientId, passwordUpdate);
            switch (PasswordUpdateResult)
            {
                case "Success": return Ok("Пароль успешно изменен");
                case "IncorrectPassword": return BadRequest("Введен неверный пароль");
                case "NewPasswordSameAsOld": return BadRequest("Новый пароль не должен совпадать со старым");
                default: return BadRequest("Произошла неизвестная ошибка, попробуйте позже");
            }
        }
    }
}
