namespace FoodDelivery.Domain.Models
{
    public class Client
    {
        /// <summary>
        /// Уникальный идентификатор клиента.
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Имя пользователя клиента для авторизации.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Пароль клиента для авторизации.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Имя клиента (опционально).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Контактный телефон клиента (опционально).
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Время последнего обновления пароля клиента.
        /// </summary>
        public DateTime LastPasswordUpdateTime { get; set; }

        /// <summary>
        /// Роль клиента в системе.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Список заказов, связанных с клиентом.
        /// </summary>
        public ICollection<Order> Orders { get; set; }

        /// <summary>
        /// Список элементов корзины, связанных с клиентом.
        /// </summary>
        public ICollection<CartItem> CartItems { get; set; }
    }
}
