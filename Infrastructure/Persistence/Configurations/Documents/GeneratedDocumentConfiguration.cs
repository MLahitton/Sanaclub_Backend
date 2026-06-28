using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanaclub.Domain.Documents;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Infrastructure.Persistence.Configurations.Documents;

public sealed class GeneratedDocumentConfiguration : IEntityTypeConfiguration<GeneratedDocument>
{
    public void Configure(EntityTypeBuilder<GeneratedDocument> builder)
    {
        builder.ToTable("generated_documents", "documents");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.DocumentKind)
            .HasColumnName("document_kind")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(x => x.SourceEntityId)
            .HasColumnName("source_entity_id")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.StoragePath)
            .HasColumnName("storage_path")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.FileSizeBytes)
            .HasColumnName("file_size_bytes")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.GeneratedAtUtc)
            .HasColumnName("generated_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.GeneratedByUserId)
            .HasColumnName("generated_by_user_id")
            .HasColumnType("uuid")
            .IsRequired();

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
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.UpdatedByUserId)
            .HasColumnName("updated_by_user_id")
            .HasColumnType("uuid")
            .IsRequired(false);

        builder.HasIndex(x => x.PatientId);
        builder.HasIndex(x => x.SourceEntityId);
        builder.HasIndex(x => x.DocumentKind);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.GeneratedAtUtc);
        builder.HasIndex(x => x.IsActive);

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
