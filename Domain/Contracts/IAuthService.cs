using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.AuthDTO;

namespace FoodDelivery.Domain.Contracts
{
    /// <summary>
    /// Сервис аутентификации и регистрации пользователей
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Регистрирует нового клиента в системе
        /// </summary>
        /// <param name="dto">Данные для регистрации нового клиента</param>
        /// <returns>Токены доступа и обновления при успешной регистрации, null если пользователь уже существует</returns>
        Task<AuthTokenResponseDTO?> RegisterAsync(RegisterDTO dto);

        /// <summary>
        /// Выполняет вход пользователя в систему
        /// </summary>
        /// <param name="dto">Учетные данные пользователя</param>
        /// <returns>Токены доступа и обновления при успешной аутентификации, null при неверных учетных данных</returns>
        Task<AuthTokenResponseDTO?> LoginAsync(LoginDTO dto);
    }
}
