using FoodDelivery.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDelivery.Storage.EntityConfigurations
{
    public class DishCategoryConfiguration : IEntityTypeConfiguration<DishCategory>
    {
        public void Configure(EntityTypeBuilder<DishCategory> builder)
        {
            builder.HasKey(dc => dc.CategoryId);

            builder.Property(dc => dc.Name)
                .IsRequired()
                .HasMaxLength(16);

            builder.HasIndex(c => c.Name)
                .IsUnique();

            builder.HasMany(dc => dc.Dishes)
                .WithOne(d => d.Category)
                .HasForeignKey(d => d.CategoryId);
        }
    }
}
