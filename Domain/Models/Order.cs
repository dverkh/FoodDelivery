namespace FoodDelivery.Domain.Models
{
    public class Order
    {
        /// <summary>
        /// Уникальный идентификатор заказа.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Идентификатор клиента, сделавшего заказ.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Статус заказа.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Общая стоимость заказа.
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Дата и время создания заказа.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Адрес доставки заказа.
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// Идентификатор курьера, назначенного на доставку заказа (опционально).
        /// </summary>
        public int? CourierId { get; set; }

        /// <summary>
        /// Ссылка на клиента, сделавшего заказ.
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// Список деталей заказа, связанных с данным заказом.
        /// </summary>
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
