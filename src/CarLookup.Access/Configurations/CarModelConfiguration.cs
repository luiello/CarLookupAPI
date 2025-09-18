using CarLookup.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarLookup.Access.Configurations;

/// <summary>
/// EF configuration for CarModel entity
/// </summary>
public class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
{
    public void Configure(EntityTypeBuilder<CarModel> builder)
    {
        builder.ToTable("CarModels");

        // PK
        builder.HasKey(cm => cm.ModelId);

        builder.Property(cm => cm.ModelId)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(cm => cm.MakeId)
            .IsRequired();

        builder.Property(cm => cm.Name)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(cm => cm.ModelYear)
            .IsRequired();

        builder.Property(cm => cm.CreatedAt)
            .IsRequired();

        // UK: Car Make Id, Model name and year
        builder.HasIndex(cm => new { cm.MakeId, cm.Name, cm.ModelYear })
            .IsUnique()
            .HasDatabaseName("IX_CarModels_MakeId_Name_Year_Unique");

        // FK: Car Model belongs to one Car Make
        builder.HasOne(cm => cm.Make)
            .WithMany(carMake => carMake.Models)
            .HasForeignKey(cm => cm.MakeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}