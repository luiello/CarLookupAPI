using CarLookup.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarLookup.Access.Configurations;

/// <summary>
/// Entity Framework configuration for UserRole junction entity
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        // PK
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Property(ur => ur.UserId)
            .IsRequired()
            .HasColumnType("char(36)");

        builder.Property(ur => ur.RoleId)
            .IsRequired()
            .HasColumnType("char(36)");

        // FK: UserRoles belongs to one User
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK: UserRoles belongs to one Role
        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ur => ur.AssignedAt)
            .IsRequired()
            .HasColumnType("datetime(6)");
    }
}