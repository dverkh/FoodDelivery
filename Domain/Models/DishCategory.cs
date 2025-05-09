namespace FoodDelivery.Domain.Models
{
    public class DishCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public ICollection<Dish> Dishes { get; set; }
    }
}
