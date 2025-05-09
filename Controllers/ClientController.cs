using System.Security.Claims;
using FoodDelivery.Domain.Contracts;
using FoodDelivery.DTO.ClientDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        private int GetClientId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetClient()
        {
            var clientId = GetClientId();
            var client = await _clientService.GetClientAsync(clientId);
            return Ok(client);
        }

        [HttpPut("name")]
        public async Task<IActionResult> UpdateName([FromBody] string newName)
        {
            var clientId = GetClientId();
            await _clientService.UpdateNameAsync(clientId, newName);
            return Ok();
        }

        [HttpPut("phone")]
        public async Task<IActionResult> UpdatePhone([FromBody] string newPhone)
        {
            var clientId = GetClientId();
            await _clientService.UpdatePhoneAsync(clientId, newPhone);
            return Ok();
        }
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
