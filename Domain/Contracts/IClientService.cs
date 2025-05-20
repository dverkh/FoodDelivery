using FoodDelivery.DTO.ClientDTO;

namespace FoodDelivery.Domain.Contracts
{
    /// <summary>
    /// Сервис для управления данными клиента в системе
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Получает информацию о клиенте по его идентификатору
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <returns>Список данных клиента в формате DTO</returns>
        Task<List<ClientResponseDTO>> GetClientAsync(int clientId);

        /// <summary>
        /// Обновляет имя клиента в системе
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="newName">Новое имя клиента</param>
        /// <returns>Задача, представляющая асинхронную операцию обновления</returns>
        Task UpdateNameAsync(int clientId, string newName);

        /// <summary>
        /// Обновляет номер телефона клиента в системе
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="phone">Новый номер телефона клиента</param>
        /// <returns>Задача, представляющая асинхронную операцию обновления</returns>
        Task UpdatePhoneAsync(int clientId, string phone);

        /// <summary>
        /// Обновляет пароль клиента после проверки текущего пароля
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <param name="dto">DTO с текущим и новым паролем</param>
        /// <returns>Строка с результатом операции обновления пароля</returns>
        Task<string> UpdatePasswordAsync(int clientId, PasswordUpdateDTO dto);
    }
}
