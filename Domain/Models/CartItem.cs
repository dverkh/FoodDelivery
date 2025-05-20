namespace FoodDelivery.Domain.Models
{
    public class CartItem
    {
        /// <summary>
        /// Идентификатор клиента, которому принадлежит элемент корзины.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Идентификатор блюда, добавленного в корзину.
        /// </summary>
        public int DishId { get; set; }

        /// <summary>
        /// Количество выбранного блюда в корзине.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Ссылка на клиента, которому принадлежит элемент корзины.
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// Ссылка на блюдо, добавленное в корзину.
        /// </summary>
        public Dish Dish { get; set; }
    }
}
