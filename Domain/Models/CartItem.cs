namespace FoodDelivery.Domain.Models
{
    public class CartItem
    {
        public int ClientId { get; set; }
        public int DishId { get; set; }
        public int Quantity { get; set; }

        public Client Client { get; set; }
        public Dish Dish { get; set; }
    }
}
