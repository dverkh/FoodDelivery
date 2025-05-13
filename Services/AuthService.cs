using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FoodDelivery.Domain.Contracts;
using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.AuthDTO;
using FoodDelivery.Storage;
using FoodDelivery.Storage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FoodDelivery.Services
{
    public class AuthService : IAuthService
    {
        private readonly FoodDeliveryContext _context;
        private readonly IAuthTokenService _authTokenService;

        public AuthService(FoodDeliveryContext context, IAuthTokenService AuthTokenService)
        {
            _context = context;
            _authTokenService = AuthTokenService;
        }

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

            var Claims = _authTokenService.GetClaims(client);
            var accessToken = _authTokenService.GenerateAccessToken(Claims);
            var refreshToken = _authTokenService.GenerateRefreshToken(Claims);

            return new AuthTokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthTokenResponseDTO?> LoginAsync(LoginDTO dto)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Username == dto.Username);

            if (client == null || !BCrypt.Net.BCrypt.Verify(dto.Password, client.Password))
                return null;

            var Claims = _authTokenService.GetClaims(client);
            var accessToken = _authTokenService.GenerateAccessToken(Claims); 
            var refreshToken = _authTokenService.GenerateRefreshToken(Claims);

            var response = new AuthTokenResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return response;
        }
    }
}
