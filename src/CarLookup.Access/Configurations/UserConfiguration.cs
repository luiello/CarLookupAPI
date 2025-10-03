using CarLookup.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarLookup.Access.Configurations;

/// <summary>
/// EF configuration for User entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // PK
        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .IsRequired()
            .HasColumnType("char(36)");

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnType("varchar(50)")
            .UseCollation("utf8mb4_unicode_ci");

        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("varchar(255)");

        builder.Property(u => u.Salt)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("varchar(255)");

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnType("varchar(255)")
            .UseCollation("utf8mb4_unicode_ci");

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true)
            .HasColumnType("tinyint(1)"); // MySQL boolean

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");

        builder.Property(u => u.UpdatedAt)
            .HasColumnType("datetime(6)");
    }
}