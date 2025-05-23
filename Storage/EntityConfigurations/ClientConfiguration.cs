using FoodDelivery.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDelivery.Storage.EntityConfigurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.ClientId);

            builder.Property(c => c.Username)
                .IsRequired()
                .HasMaxLength(16);

            builder.HasIndex(c => c.Username)
                .IsUnique();

            builder.Property(c => c.Password)
                .IsRequired()
                .HasMaxLength(60);

            builder.Property(c => c.Name)
                .IsRequired(false)
                .HasMaxLength(50);

            builder.Property(c => c.Phone)
                .IsRequired(false)
                .HasMaxLength(20);

            builder.HasMany(c => c.Orders)
                .WithOne(o => o.Client)
                .HasForeignKey(o => o.ClientId);
        }
    }
}
