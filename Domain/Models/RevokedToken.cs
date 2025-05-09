namespace FoodDelivery.Domain.Models
{
    public class RevokedToken
    {
        public int Id { get; set; }
        public string Jti { get; set; }
        public DateTime Expiration { get; set; }
    }
}
