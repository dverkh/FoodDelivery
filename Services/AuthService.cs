using FoodDelivery.Domain.Contracts;
using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.AuthDTO;
using FoodDelivery.Storage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services
{
    /// <summary>
    /// Реализация сервиса аутентификации и регистрации пользователей
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly FoodDeliveryContext _context;
        private readonly IAuthTokenService _authTokenService;

        /// <summary>
        /// Инициализирует новый экземпляр сервиса аутентификации
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="AuthTokenService">Сервис для работы с токенами</param>
        public AuthService(FoodDeliveryContext context, IAuthTokenService AuthTokenService)
        {
            _context = context;
            _authTokenService = AuthTokenService;
        }

        /// <summary>
        /// Регистрирует нового клиента в системе
        /// </summary>
        /// <param name="dto">Данные для регистрации нового клиента</param>
        /// <returns>Токены доступа и обновления при успешной регистрации, null если пользователь уже существует</returns>
        public async Task<AuthTokenResponseDTO?> RegisterAsync(RegisterDTO dto)
        {
            var existingUser = await _context.Clients
                .FirstOrDefaultAsync(c => c.Username == dto.Username);
            if (existingUser != null) return null;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var client = new Client
            {
                Username = dto.Username,
                Password = hashedPassword,
                Name = dto.Name,
                Phone = dto.Phone,
                LastPasswordUpdateTime = DateTime.UtcNow,
                Role = "Client"
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var claims = _authTokenService.GetClaims(client);
            var accessToken = _authTokenService.GenerateAccessToken(claims);
            var refreshToken = _authTokenService.GenerateRefreshToken(claims);

            return new AuthTokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        /// <summary>
        /// Выполняет вход пользователя в систему
        /// </summary>
        /// <param name="dto">Учетные данные пользователя</param>
        /// <returns>Токены доступа и обновления при успешной аутентификации, null при неверных учетных данных</returns>
        public async Task<AuthTokenResponseDTO?> LoginAsync(LoginDTO dto)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Username == dto.Username);

            if (client == null || !BCrypt.Net.BCrypt.Verify(dto.Password, client.Password))
                return null;

            var claims = _authTokenService.GetClaims(client);
            var accessToken = _authTokenService.GenerateAccessToken(claims); 
            var refreshToken = _authTokenService.GenerateRefreshToken(claims);

            var response = new AuthTokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return response;
        }
    }
}
