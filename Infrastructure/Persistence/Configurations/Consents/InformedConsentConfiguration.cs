using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanaclub.Domain.Auth;
using Sanaclub.Domain.Catalogs;
using Sanaclub.Domain.Consents;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Infrastructure.Persistence.Configurations.Consents;

public sealed class InformedConsentConfiguration : IEntityTypeConfiguration<InformedConsent>
{
    public void Configure(EntityTypeBuilder<InformedConsent> builder)
    {
        builder.ToTable("informed_consents", "clinical");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.DocumentTypeId)
            .HasColumnName("document_type_id")
            .IsRequired();

        builder.Property(x => x.ConsentStatusId)
            .HasColumnName("consent_status_id")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.SignedAtUtc)
            .HasColumnName("signed_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.SignedByUserId)
            .HasColumnName("signed_by_user_id")
            .IsRequired(false);

        builder.Property(x => x.PatientSignerName)
            .HasColumnName("patient_signer_name")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

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

        builder.HasIndex(x => x.PatientId);
        builder.HasIndex(x => x.DocumentTypeId);
        builder.HasIndex(x => x.ConsentStatusId);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<DocumentType>()
            .WithMany()
            .HasForeignKey(x => x.DocumentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ConsentStatus>()
            .WithMany()
            .HasForeignKey(x => x.ConsentStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.SignedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

