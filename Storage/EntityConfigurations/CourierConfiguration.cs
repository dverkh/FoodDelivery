using FoodDelivery.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDelivery.Storage.EntityConfigurations
{
    public class CourierConfiguration
    {
        public void Configure(EntityTypeBuilder<Courier> builder)
        {
            builder.HasKey(c => c.CourierId);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(10);
        }
    }
}
