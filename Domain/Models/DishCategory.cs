namespace FoodDelivery.Domain.Models
{
    public class DishCategory
    {
        /// <summary>
        /// Уникальный идентификатор категории блюда.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Название категории блюда.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Список блюд, относящихся к данной категории.
        /// </summary>
        public ICollection<Dish> Dishes { get; set; }
    }
}
