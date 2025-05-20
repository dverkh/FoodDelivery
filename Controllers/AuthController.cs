using System.IdentityModel.Tokens.Jwt;
using FoodDelivery.Domain.Contracts;
using FoodDelivery.DTO.AuthDTO;
using FoodDelivery.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FoodDelivery.Controllers
{
    /// <summary>
    /// Контроллер для управления аутентификацией пользователей
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAuthTokenService _authTokenService;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера аутентификации
        /// </summary>
        /// <param name="authService">Сервис аутентификации</param>
        /// <param name="authTokenService">Сервис управления токенами</param>
        public AuthController(IAuthService authService, IAuthTokenService authTokenService)
        {
            _authService = authService;
            _authTokenService = authTokenService;
        }

        /// <summary>
        /// Регистрирует нового пользователя в системе
        /// </summary>
        /// <param name="dto">Данные для регистрации</param>
        /// <returns>Токены доступа при успешной регистрации или ошибку, если пользователь существует</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var tokens = await _authService.RegisterAsync(dto);
            if (tokens == null) return BadRequest("Пользователь уже существует");
            return Ok(tokens);
        }

        /// <summary>
        /// Выполняет вход пользователя в систему
        /// </summary>
        /// <param name="dto">Учетные данные пользователя</param>
        /// <returns>Токены доступа при успешной аутентификации или ошибку при неверных учетных данных</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var tokens = await _authService.LoginAsync(dto);
            if (tokens == null) return Unauthorized("Неверный логин или пароль");
            return Ok(tokens);
        }

        /// <summary>
        /// Выполняет выход пользователя из системы
        /// </summary>
        /// <param name="refreshToken">Токен обновления для отзыва</param>
        /// <returns>Успешный результат операции</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            await _authTokenService.RevokeTokenAsync(refreshToken);
            return Ok();
        }

        /// <summary>
        /// Обновляет токен доступа с помощью токена обновления
        /// </summary>
        /// <param name="refreshToken">Токен обновления</param>
        /// <returns>Новые токены доступа или ошибку при невалидном токене</returns>
        [HttpPost("tokenRefresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (await _authTokenService.IsTokenRevokedAsync(refreshToken))
            {
                return Unauthorized("Отозванный токен");
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidateAudience = true,
                    ValidAudience = AuthOptions.AUDIENCE,
                    ValidateLifetime = true,
                    IssuerSigningKey = AuthOptions.RefreshToken.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true
                }, out SecurityToken validatedToken);

                var сlaims = principal.Claims.ToList();

                if (!await _authTokenService.ValidateTokenClaimsAsync(сlaims))
                {
                    return Unauthorized("Невалидный токен");
                }

                await _authTokenService.RevokeTokenAsync(refreshToken);

                await _authTokenService.CleanupExpiredTokensAsync();

                return Ok(new
                {
                    AccessToken = _authTokenService.GenerateAccessToken(сlaims),
                    RefreshToken = _authTokenService.GenerateRefreshToken(сlaims)
                });
            }
            catch
            {
                return Unauthorized("Неизвестная ошибка");
            }
        }
    }
}
