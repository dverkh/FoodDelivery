namespace FoodDelivery.Domain.Models
{
    public class Dish
    {
        /// <summary>
        /// Уникальный идентификатор блюда.
        /// </summary>
        public int DishId { get; set; }

        /// <summary>
        /// Название блюда.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор категории, к которой относится блюдо.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Описание блюда.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Цена блюда.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Флаг, указывающий, доступно ли блюдо для заказа.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Ссылка на категорию, к которой относится блюдо.
        /// </summary>
        public DishCategory Category { get; set; }

        /// <summary>
        /// Список деталей заказов, связанных с данным блюдом.
        /// </summary>
        public ICollection<OrderDetails> OrderDetails { get; set; }

        /// <summary>
        /// Список элементов корзины, связанных с данным блюдом.
        /// </summary>
        public ICollection<CartItem> CartItems { get; set; }
    }
}
