using System.IdentityModel.Tokens.Jwt;
using FoodDelivery.Domain.Contracts;
using FoodDelivery.DTO.AuthDTO;
using FoodDelivery.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FoodDelivery.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAuthTokenService _authTokenService;

        public AuthController(IAuthService authService, IAuthTokenService authTokenService)
        {
            _authService = authService;
            _authTokenService = authTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var tokens = await _authService.RegisterAsync(dto);
            if (tokens == null) return BadRequest("Пользователь уже существует");
            return Ok(tokens);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var tokens = await _authService.LoginAsync(dto);
            if (tokens == null) return Unauthorized("Неверный логин или пароль");
            return Ok(tokens);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            await _authTokenService.RevokeTokenAsync(refreshToken);
            return Ok();
        }

        [HttpPost("refresh")]
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
                return Unauthorized("Невалидный токен");
            }
        }
    }
}
