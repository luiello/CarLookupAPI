using CarLookup.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarLookup.Access.Configurations;

/// <summary>
/// EF configuration for Role entity
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        // PK
        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.RoleId)
            .IsRequired()
            .HasColumnType("char(36)");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("varchar(50)")
            .UseCollation("utf8mb4_unicode_ci");

        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.Property(r => r.Description)
            .HasMaxLength(255)
            .HasColumnType("varchar(255)")
            .UseCollation("utf8mb4_unicode_ci");

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(r => r.UpdatedAt)
            .HasColumnType("datetime(6)");
    }
}