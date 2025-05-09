using FoodDelivery.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDelivery.Storage.EntityConfigurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.OrderId);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(o => o.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(o => o.DateTime)
                .IsRequired();

            builder.Property(o => o.DeliveryAddress)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientId);

            builder.HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);
        }
    }
}
