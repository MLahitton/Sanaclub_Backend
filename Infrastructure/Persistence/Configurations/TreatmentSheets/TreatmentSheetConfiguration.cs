using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Infrastructure.Persistence.Configurations.TreatmentSheets;

public sealed class TreatmentSheetConfiguration : IEntityTypeConfiguration<TreatmentSheet>
{
    public void Configure(EntityTypeBuilder<TreatmentSheet> builder)
    {
        builder.ToTable("treatment_sheets", "clinical");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.TreatmentStatusId)
            .HasColumnName("treatment_status_id")
            .IsRequired();

        builder.Property(x => x.TreatmentNumber)
            .HasColumnName("treatment_number")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.ConsultationDate)
            .HasColumnName("consultation_date")
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(x => x.EpsTreatingDoctorDiagnosis)
            .HasColumnName("eps_treating_doctor_diagnosis")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.ReferredClinicalHistory)
            .HasColumnName("referred_clinical_history")
            .HasMaxLength(4000)
            .IsRequired(false);

        builder.Property(x => x.IndicationDate)
            .HasColumnName("indication_date")
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

        builder.Property(x => x.NervousSystemIndications)
            .HasColumnName("nervous_system_indications")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.DecompressSpine)
            .HasColumnName("decompress_spine")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DecompressNeck)
            .HasColumnName("decompress_neck")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DecompressBack)
            .HasColumnName("decompress_back")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.EndocrineNerves)
            .HasColumnName("endocrine_nerves")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.EndocrineDefenses)
            .HasColumnName("endocrine_defenses")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.EndocrineHormones)
            .HasColumnName("endocrine_hormones")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CardiovascularReflexologyWith)
            .HasColumnName("cardiovascular_reflexology_with")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.DigestiveColonReflexologyWith)
            .HasColumnName("digestive_colon_reflexology_with")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.RespiratoryReflexologyWith)
            .HasColumnName("respiratory_reflexology_with")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.UrinaryReflexologyWithAcidFruits)
            .HasColumnName("urinary_reflexology_with_acid_fruits")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.OtherIndications)
            .HasColumnName("other_indications")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.Observations)
            .HasColumnName("observations")
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(x => x.ApprovedAtUtc)
            .HasColumnName("approved_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.ApprovedByUserId)
            .HasColumnName("approved_by_user_id")
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
            .IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.UpdatedByUserId)
            .HasColumnName("updated_by_user_id")
            .IsRequired(false);

        builder.HasIndex(x => x.PatientId);
        builder.HasIndex(x => x.TreatmentStatusId);
        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasOne<Domain.Patients.Patient>()
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.Catalogs.TreatmentStatus>()
            .WithMany()
            .HasForeignKey(x => x.TreatmentStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
