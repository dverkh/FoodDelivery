namespace FoodDelivery.DTO.OrderDTO
{
    public class OrderItemDTO
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
