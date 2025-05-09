namespace FoodDelivery.Domain.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
        public string DeliveryAddress { get; set; }
        public int? CourierId { get; set; }

        public Client Client { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
