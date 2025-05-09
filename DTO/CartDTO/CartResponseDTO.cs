namespace FoodDelivery.DTO.CartDTO
{
    public class CartResponseDTO
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public decimal DishPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total => DishPrice * Quantity;
    }
}
