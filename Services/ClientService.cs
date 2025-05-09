using FoodDelivery.Domain.Contracts;
using FoodDelivery.Domain.Models;
using FoodDelivery.DTO.ClientDTO;
using FoodDelivery.Storage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Services
{
    public class ClientService : IClientService
    {
        private readonly FoodDeliveryContext _context;

        public ClientService(FoodDeliveryContext context)
        {
            _context = context;
        }

        public async Task<List<ClientResponseDTO>> GetClientAsync(int clientId)
        {
            return await _context.Clients
                .Where(c => c.ClientId == clientId)
                .Select(c => new ClientResponseDTO
                {
                    Username = c.Username,
                    Name = c.Name,
                    Phone = c.Phone,
                })
                .ToListAsync();
        }

        public async Task UpdateNameAsync(int clientId, string newName)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client != null)
            {
                client.Name = newName;
            }

            await _context.SaveChangesAsync();

        }

        public async Task UpdatePhoneAsync(int clientId, string newPhone)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client != null)
            {
                client.Phone = newPhone;
            }

            await _context.SaveChangesAsync();

        }

        public async Task<string> UpdatePasswordAsync(int clientId, PasswordUpdateDTO dto)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client != null)
            {
                bool isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.oldPassword, client.Password);
                if (!isOldPasswordCorrect)
                {
                    return "IncorrectPassword";
                }
                if (dto.newPassword == dto.oldPassword)
                {
                    return "NewPasswordSameAsOld";
                }
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.newPassword);
                client.Password = hashedPassword;

                client.LastPasswordUpdateTime = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return "Success";

        }
    }
}
