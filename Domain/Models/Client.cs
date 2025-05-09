namespace FoodDelivery.Domain.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public DateTime LastPasswordUpdateTime {get; set; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
