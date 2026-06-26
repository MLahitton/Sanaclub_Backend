using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanaclub.Domain.Catalogs;
using Sanaclub.Domain.Patients;

namespace Sanaclub.Infrastructure.Persistence.Configurations.Patients;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients", "clinical");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.IdentificationTypeId)
            .HasColumnName("identification_type_id")
            .IsRequired();

        builder.Property(x => x.IdentificationNumber)
            .HasColumnName("identification_number")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.BirthDate)
            .HasColumnName("birth_date")
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(x => x.GenderId)
            .HasColumnName("gender_id")
            .IsRequired(false);

        builder.Property(x => x.CivilStatusId)
            .HasColumnName("civil_status_id")
            .IsRequired(false);

        builder.Property(x => x.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(x => x.Address)
            .HasColumnName("address")
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.CityOrMunicipality)
            .HasColumnName("city_or_municipality")
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(x => x.Occupation)
            .HasColumnName("occupation")
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(x => x.EmergencyContactName)
            .HasColumnName("emergency_contact_name")
            .HasMaxLength(200)
            .IsRequired(false);

        builder.Property(x => x.EmergencyContactRelationship)
            .HasColumnName("emergency_contact_relationship")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.EmergencyContactPhone)
            .HasColumnName("emergency_contact_phone")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.PatientStatusId)
            .HasColumnName("patient_status_id")
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
            .IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(x => x.UpdatedByUserId)
            .HasColumnName("updated_by_user_id")
            .IsRequired(false);

        builder.HasIndex(x => new { x.IdentificationTypeId, x.IdentificationNumber })
            .IsUnique();

        builder.HasIndex(x => x.LastName);
        builder.HasIndex(x => x.IdentificationNumber);
        builder.HasIndex(x => x.PatientStatusId);
        builder.HasIndex(x => x.IsActive);

        builder.HasOne<IdentificationType>()
            .WithMany()
            .HasForeignKey(x => x.IdentificationTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Gender>()
            .WithMany()
            .HasForeignKey(x => x.GenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<CivilStatus>()
            .WithMany()
            .HasForeignKey(x => x.CivilStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PatientStatus>()
            .WithMany()
            .HasForeignKey(x => x.PatientStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
