namespace FoodDelivery.DTO.OrderDTO
{
    public class OrderListDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
        public string DeliveryAddress { get; set; }
    }
}
