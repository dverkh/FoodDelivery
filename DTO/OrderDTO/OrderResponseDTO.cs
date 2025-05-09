namespace FoodDelivery.DTO.OrderDTO
{
    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
        public string DeliveryAddress { get; set; }
        public List<OrderItemDTO> Items { get; set; }
    }
}
