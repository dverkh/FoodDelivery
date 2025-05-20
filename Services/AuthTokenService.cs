using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodDelivery.Domain.Contracts;
using FoodDelivery.Domain.Models;
using FoodDelivery.Storage;
using FoodDelivery.Storage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FoodDelivery.Services
{
    /// <summary>
    /// Реализация сервиса для работы с токенами аутентификации
    /// </summary>
    public class AuthTokenService : IAuthTokenService
    {
        private readonly FoodDeliveryContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр сервиса токенов
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public AuthTokenService(FoodDeliveryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает список утверждений (claims) для пользователя
        /// </summary>
        /// <param name="user">Данные клиента</param>
        /// <returns>Список утверждений для генерации токенов</returns>
        public List<Claim> GetClaims(Client user)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ClientId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
        }

        /// <summary>
        /// Генерирует токен доступа на основе утверждений
        /// </summary>
        /// <param name="claims">Список утверждений</param>
        /// <returns>Токен доступа</returns>
        public string GenerateAccessToken(List<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(AuthOptions.AccessToken.LifeTime),
                signingCredentials: new SigningCredentials
                (
                    AuthOptions.AccessToken.GetSymmetricSecurityKey(), 
                    SecurityAlgorithms.HmacSha256
                )
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Генерирует токен обновления на основе утверждений
        /// </summary>
        /// <param name="claims">Список утверждений</param>
        /// <returns>Токен обновления</returns>
        public string GenerateRefreshToken(List<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(AuthOptions.RefreshToken.LifeTime),
                signingCredentials: new SigningCredentials
                (
                    AuthOptions.RefreshToken.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256
                )
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Проверяет валидность утверждений токена
        /// </summary>
        /// <param name="claims">Список утверждений для проверки</param>
        /// <returns>true если утверждения валидны, иначе false</returns>
        public async Task<bool> ValidateTokenClaimsAsync(List<Claim> claims)
        {
            var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var iatClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat);

            if (userIdClaim == null || iatClaim == null)
                return false;

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return false;

            var user = await _context.Clients.FindAsync(userId);
            if (user == null)
                return false;

            if (!long.TryParse(iatClaim.Value, out var iatSeconds))
                return false;

            var tokenIssuedAt = DateTimeOffset.FromUnixTimeSeconds(iatSeconds).UtcDateTime;

            if (user.LastPasswordUpdateTime > tokenIssuedAt)
                return false;

            return true;
        }

        /// <summary>
        /// Отзывает токен
        /// </summary>
        /// <param name="token">Токен для отзыва</param>
        public async Task RevokeTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expiration = jwtToken.ValidTo;

            await _context.RevokedTokens.AddAsync(new RevokedToken
            {
                Jti = jti,
                Expiration = expiration,
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Проверяет, был ли токен отозван
        /// </summary>
        /// <param name="token">Токен для проверки</param>
        /// <returns>true если токен отозван, иначе false</returns>
        public async Task<bool> IsTokenRevokedAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            return await _context.RevokedTokens
                .AnyAsync(t => t.Jti == jti && t.Expiration > DateTime.UtcNow);
        }

        /// <summary>
        /// Очищает истекшие отозванные токены из базы данных
        /// </summary>
        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = _context.RevokedTokens
                .Where(t => t.Expiration <= DateTime.UtcNow);

            _context.RevokedTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}
