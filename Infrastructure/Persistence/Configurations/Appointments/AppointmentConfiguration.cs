using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanaclub.Domain.Appointments;
using Sanaclub.Domain.Auth;
using Sanaclub.Domain.Catalogs;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Infrastructure.Persistence.Configurations.Appointments;

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments", "clinical");
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

        builder.Property(x => x.ClinicalReferenceType)
            .HasColumnName("clinical_reference_type")
            .HasMaxLength(Appointment.ClinicalReferenceTypeMaxLength)
            .IsRequired();

        builder.Property(x => x.ClinicalReferenceId)
            .HasColumnName("clinical_reference_id")
            .IsRequired();

        builder.Property(x => x.TherapistUserId)
            .HasColumnName("therapist_user_id")
            .IsRequired();

        builder.Property(x => x.AppointmentDate)
            .HasColumnName("appointment_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.StartTime)
            .HasColumnName("start_time")
            .HasColumnType("time")
            .IsRequired();

        builder.Property(x => x.EndTime)
            .HasColumnName("end_time")
            .HasColumnType("time")
            .IsRequired();

        builder.Property(x => x.AppointmentStatusId)
            .HasColumnName("appointment_status_id")
            .IsRequired();

        builder.Property(x => x.PatientNameSnapshot)
            .HasColumnName("patient_name_snapshot")
            .HasMaxLength(Appointment.PatientNameSnapshotMaxLength)
            .IsRequired(false);

        builder.Property(x => x.ScheduledByUserId)
            .HasColumnName("scheduled_by_user_id")
            .IsRequired();

        builder.Property(x => x.ConfirmedAtUtc)
            .HasColumnName("confirmed_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.ConfirmedByUserId)
            .HasColumnName("confirmed_by_user_id")
            .IsRequired(false);

        builder.Property(x => x.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.CancelledByUserId)
            .HasColumnName("cancelled_by_user_id")
            .IsRequired(false);

        builder.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasMaxLength(Appointment.NotesMaxLength)
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

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TreatmentSheet>()
            .WithMany()
            .HasForeignKey(x => x.TreatmentSheetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.TherapistUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppointmentStatus>()
            .WithMany()
            .HasForeignKey(x => x.AppointmentStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PatientId);
        builder.HasIndex(x => x.TreatmentSheetId);
        builder.HasIndex(x => new { x.TherapistUserId, x.AppointmentDate });
        builder.HasIndex(x => x.AppointmentStatusId);
        builder.HasIndex(x => new { x.ClinicalReferenceType, x.ClinicalReferenceId });
        builder.HasIndex(x => new { x.TherapistUserId, x.AppointmentDate, x.StartTime, x.EndTime });
        builder.HasIndex(x => x.IsActive);
    }
}
