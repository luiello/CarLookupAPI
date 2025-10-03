using CarLookup.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarLookup.Access.Configurations;

/// <summary>
/// EF configuration for CarMake entity
/// </summary>
public class CarMakeConfiguration : IEntityTypeConfiguration<CarMake>
{
    public void Configure(EntityTypeBuilder<CarMake> builder)
    {
        builder.ToTable("CarMakes");

        // PK
        builder.HasKey(cm => cm.MakeId);

        builder.Property(cm => cm.MakeId)
            .IsRequired()
            .ValueGeneratedNever()
            .HasColumnType("char(36)"); // MySQL GUID format

        builder.Property(cm => cm.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)")
            .UseCollation("utf8mb4_unicode_ci"); // Case-insensitive Unicode collation

        builder.Property(cm => cm.CountryOfOrigin)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("varchar(100)")
            .UseCollation("utf8mb4_unicode_ci");

        builder.Property(cm => cm.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)"); // MySQL datetime with microsecond precision

        builder.Property(cm => cm.UpdatedAt)
            .HasColumnType("datetime(6)");

        // UK: Car Make name
        builder.HasIndex(cm => cm.Name)
            .IsUnique()
            .HasDatabaseName("IX_CarMakes_Name_Unique");

        // FK: Car Make has many Car Models
        builder.HasMany(cm => cm.Models)
            .WithOne(carModel => carModel.Make)
            .HasForeignKey(carModel => carModel.MakeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}