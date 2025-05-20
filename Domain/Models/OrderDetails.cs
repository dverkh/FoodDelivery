namespace FoodDelivery.Domain.Models
{
    public class OrderDetails
    {
        /// <summary>
        /// Идентификатор заказа, к которому относятся детали.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Идентификатор блюда, включенного в заказ.
        /// </summary>
        public int DishId { get; set; }

        /// <summary>
        /// Количество блюда в заказе.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Ссылка на заказ, к которому относятся детали.
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// Ссылка на блюдо, включенное в заказ.
        /// </summary>
        public Dish Dish { get; set; }
    }
}
