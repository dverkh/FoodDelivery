using FoodDelivery.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDelivery.Storage.EntityConfigurations
{
    public class DishConfiguration : IEntityTypeConfiguration<Dish>
    {
        public void Configure(EntityTypeBuilder<Dish> builder)
        {
            builder.HasKey(d => d.DishId);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.Description)
                .HasMaxLength(500);

            builder.Property(d => d.Price)
                .HasColumnType("decimal(10,2)");

            builder.Property(d => d.IsAvailable)
                .IsRequired();

            builder.HasOne(d => d.Category)
                .WithMany(c => c.Dishes)
                .HasForeignKey(d => d.CategoryId);

            builder.HasMany(d => d.OrderDetails)
                .WithOne(od => od.Dish)
                .HasForeignKey(od => od.DishId);
        }
    }
}
