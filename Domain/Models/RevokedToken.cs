namespace FoodDelivery.Domain.Models
{
    public class RevokedToken
    {
    /// <summary>
    /// Уникальный идентификатор отозванного токена.
    /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Время истечения отозванного токена.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Уникальный идентификатор токена (JTI), который был отозван.
        /// </summary>
        public string Jti { get; set; }
    }
}
