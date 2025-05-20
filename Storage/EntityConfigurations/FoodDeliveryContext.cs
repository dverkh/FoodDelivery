using FoodDelivery.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.Storage.EntityConfigurations
{
    public class FoodDeliveryContext : DbContext
    {
        public FoodDeliveryContext(DbContextOptions<FoodDeliveryContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishCategory> DishCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<RevokedToken> RevokedTokens { get; set; }
        public DbSet<Courier> Couriers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FoodDeliveryContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
