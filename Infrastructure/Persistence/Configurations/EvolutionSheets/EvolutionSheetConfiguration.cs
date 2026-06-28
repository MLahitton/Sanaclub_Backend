using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanaclub.Domain.EvolutionSheets;

namespace Sanaclub.Infrastructure.Persistence.Configurations.EvolutionSheets;

public sealed class EvolutionSheetConfiguration : IEntityTypeConfiguration<EvolutionSheet>
{
    public void Configure(EntityTypeBuilder<EvolutionSheet> builder)
    {
        builder.ToTable("evolution_sheets", "clinical");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.TreatmentSheetId)
            .HasColumnName("treatment_sheet_id")
            .IsRequired();

        builder.Property(x => x.EvolutionStatusId)
            .HasColumnName("evolution_status_id")
            .IsRequired();

        builder.Property(x => x.TherapyNumber)
            .HasColumnName("therapy_number")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.EvolutionDate)
            .HasColumnName("evolution_date")
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(x => x.EntryTime)
            .HasColumnName("entry_time")
            .HasColumnType("time")
            .IsRequired(false);

        builder.Property(x => x.ExitTime)
            .HasColumnName("exit_time")
            .HasColumnType("time")
            .IsRequired(false);

        builder.Property(x => x.AssignedStaffName)
            .HasColumnName("assigned_staff_name")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(x => x.TherapyName)
            .HasColumnName("therapy_name")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(x => x.EvolutionNotes)
            .HasColumnName("evolution_notes")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(x => x.NewIndications)
            .HasColumnName("new_indications")
            .HasMaxLength(4000)
            .IsRequired(false);

        builder.Property(x => x.CompletedAtUtc)
            .HasColumnName("completed_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.CompletedByUserId)
            .HasColumnName("completed_by_user_id")
            .HasColumnType("uuid")
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
        builder.HasIndex(x => x.TreatmentSheetId);
        builder.HasIndex(x => x.EvolutionStatusId);
        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasOne<Domain.Patients.Patient>()
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.TreatmentSheets.TreatmentSheet>()
            .WithMany()
            .HasForeignKey(x => x.TreatmentSheetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Catalogs.EvolutionStatus>()
            .WithMany()
            .HasForeignKey(x => x.EvolutionStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
