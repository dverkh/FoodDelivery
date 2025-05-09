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
    public class AuthTokenService : IAuthTokenService
    {
        private readonly FoodDeliveryContext _context;
        public AuthTokenService(FoodDeliveryContext context)
        {
            _context = context;
        }
        public List<Claim> GetClaims(Client user)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.ClientId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
        }

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

        public async Task<bool> IsTokenRevokedAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            return await _context.RevokedTokens
                .AnyAsync(t => t.Jti == jti && t.Expiration > DateTime.UtcNow);
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = _context.RevokedTokens
                .Where(t => t.Expiration <= DateTime.UtcNow);

            _context.RevokedTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}
