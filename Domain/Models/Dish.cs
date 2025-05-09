namespace FoodDelivery.Domain.Models
{
    public class Dish
    {
        public int DishId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        public DishCategory Category { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
