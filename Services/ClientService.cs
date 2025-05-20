using FoodDelivery.Domain.Contracts;
using FoodDelivery.DTO.ClientDTO;
using FoodDelivery.Storage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services
{
    /// <summary>
    /// Сервис для управления данными клиента в системе
    /// </summary>
    public class ClientService : IClientService
    {
        private readonly FoodDeliveryContext _context;

        public ClientService(FoodDeliveryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает информацию о клиенте по его идентификатору
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <returns>Список данных клиента в формате DTO</returns>
        public async Task<List<ClientResponseDTO>> GetClientAsync(int clientId)
        {
            return await _context.Clients
                .Where(c => c.ClientId == clientId)
                .Select(c => new ClientResponseDTO
                {
                    Username = c.Username,
                    Name = c.Name,
                    Phone = c.Phone,
                })
                .ToListAsync();
        }

        /// <summary>
        /// Обновляет имя клиента в системе
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="newName">Новое имя клиента</param>
        /// <returns>Задача, представляющая асинхронную операцию обновления</returns>
        public async Task UpdateNameAsync(int clientId, string newName)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client != null)
            {
                client.Name = newName;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Обновляет номер телефона клиента в системе
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="newPhone">Новый номер телефона клиента</param>
        /// <returns>Задача, представляющая асинхронную операцию обновления</returns>
        public async Task UpdatePhoneAsync(int clientId, string newPhone)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client != null)
            {
                client.Phone = newPhone;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Обновляет пароль клиента после проверки текущего пароля
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="dto">DTO с текущим и новым паролем</param>
        /// <returns>Строка с результатом операции обновления пароля: "Success", "IncorrectPassword" или "NewPasswordSameAsOld"</returns>
        public async Task<string> UpdatePasswordAsync(int clientId, PasswordUpdateDTO dto)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client != null)
            {
                bool isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.oldPassword, client.Password);
                if (!isOldPasswordCorrect)
                {
                    return "IncorrectPassword";
                }
                if (dto.newPassword == dto.oldPassword)
                {
                    return "NewPasswordSameAsOld";
                }
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.newPassword);
                client.Password = hashedPassword;

                client.LastPasswordUpdateTime = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return "Success";
        }
    }
}
