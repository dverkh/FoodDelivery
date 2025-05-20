namespace FoodDelivery.Domain.Models
{
    public class Courier
    {
        /// <summary>
        /// Уникальный идентификатор курьера.
        /// </summary>
        public int CourierId { get; set; }
        /// <summary>
        /// Имя курьера.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Телефон курьера.
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
