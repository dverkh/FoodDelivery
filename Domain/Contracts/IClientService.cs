using FoodDelivery.DTO.ClientDTO;

namespace FoodDelivery.Domain.Contracts
{
    public interface IClientService
    {
        Task<List<ClientResponseDTO>> GetClientAsync(int clientId);
        Task UpdateNameAsync(int clientId, string newName);
        Task UpdatePhoneAsync(int clientId, string phone);
        Task<string> UpdatePasswordAsync(int clientId, PasswordUpdateDTO dto);
    }
}
