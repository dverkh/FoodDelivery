namespace FoodDelivery.Domain.Models
{
    public class OrderDetails
    {
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int Quantity { get; set; }

        public Order Order { get; set; }
        public Dish Dish { get; set; }
    }
}
