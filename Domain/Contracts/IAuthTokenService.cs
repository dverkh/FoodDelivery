using FoodDelivery.Domain.Models;
using System.Security.Claims;

namespace FoodDelivery.Domain.Contracts
{
    /// <summary>
    /// Сервис для работы с токенами аутентификации
    /// </summary>
    public interface IAuthTokenService
    {
        /// <summary>
        /// Получает список утверждений (claims) для пользователя
        /// </summary>
        /// <param name="user">Данные клиента</param>
        /// <returns>Список утверждений для генерации токенов</returns>
        List<Claim> GetClaims(Client user);

        /// <summary>
        /// Генерирует токен доступа на основе утверждений
        /// </summary>
        /// <param name="claims">Список утверждений</param>
        /// <returns>Токен доступа</returns>
        string GenerateAccessToken(List<Claim> claims);

        /// <summary>
        /// Генерирует токен обновления на основе утверждений
        /// </summary>
        /// <param name="claims">Список утверждений</param>
        /// <returns>Токен обновления</returns>
        string GenerateRefreshToken(List<Claim> claims);

        /// <summary>
        /// Проверяет валидность утверждений токена
        /// </summary>
        /// <param name="claims">Список утверждений для проверки</param>
        /// <returns>true если утверждения валидны, иначе false</returns>
        Task<bool> ValidateTokenClaimsAsync(List<Claim> claims);

        /// <summary>
        /// Отзывает токен
        /// </summary>
        /// <param name="token">Токен для отзыва</param>
        Task RevokeTokenAsync(string token);

        /// <summary>
        /// Проверяет, был ли токен отозван
        /// </summary>
        /// <param name="token">Токен для проверки</param>
        /// <returns>true если токен отозван, иначе false</returns>
        Task<bool> IsTokenRevokedAsync(string token);

        /// <summary>
        /// Очищает истекшие отозванные токены из базы данных
        /// </summary>
        Task CleanupExpiredTokensAsync();
    }
}
