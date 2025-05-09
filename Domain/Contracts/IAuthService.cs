using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.AuthDTO;

namespace FoodDelivery.Domain.Contracts
{
    public interface IAuthService
    {
        Task<AuthTokenResponseDTO?> RegisterAsync(RegisterDTO dto);
        Task<AuthTokenResponseDTO?> LoginAsync(LoginDTO dto);
    }
}
