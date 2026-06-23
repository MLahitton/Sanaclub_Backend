using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanaclub.Domain.Auth;

namespace Sanaclub.Infrastructure.Persistence.Configurations.Auth;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens", "auth");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.TokenHash)
            .HasColumnName("token_hash")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .HasColumnName("expires_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.RevokedAtUtc)
            .HasColumnName("revoked_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.RevokedByUserId)
            .HasColumnName("revoked_by_user_id")
            .IsRequired(false);

        builder.Property(x => x.RevocationReason)
            .HasColumnName("revocation_reason")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.ReplacedByTokenHash)
            .HasColumnName("replaced_by_token_hash")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.CreatedByIpAddress)
            .HasColumnName("created_by_ip_address")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.RevokedByIpAddress)
            .HasColumnName("revoked_by_ip_address")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.UpdatedByUserId)
            .HasColumnName("updated_by_user_id")
            .IsRequired(false);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ExpiresAtUtc);
        builder.HasIndex(x => x.RevokedAtUtc);
        builder.HasIndex(x => new { x.UserId, x.RevokedAtUtc, x.ExpiresAtUtc });
    }
}
