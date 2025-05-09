using FoodDelivery.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDelivery.Storage.EntityConfigurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {

            builder.HasKey(ci => new { ci.ClientId, ci.DishId });

            builder.Property(ci => ci.Quantity)
                .IsRequired();

            builder.HasOne(ci => ci.Client)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.ClientId);

            builder.HasOne(ci => ci.Dish)
                .WithMany(d => d.CartItems)
                .HasForeignKey(ci => ci.DishId);
        }
    }
}
