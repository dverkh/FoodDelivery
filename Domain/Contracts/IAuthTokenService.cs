using FoodDelivery.Domain.Models;
using System.Security.Claims;

namespace FoodDelivery.Domain.Contracts
{
    public interface IAuthTokenService
    {
        List<Claim> GetClaims(Client user);
        string GenerateAccessToken(List<Claim> claims);
        string GenerateRefreshToken(List<Claim> claims);
        Task<bool> ValidateTokenClaimsAsync(List<Claim> claims);
        public Task RevokeTokenAsync(string token);
        Task<bool> IsTokenRevokedAsync(string token);
        Task CleanupExpiredTokensAsync();
    }
}
