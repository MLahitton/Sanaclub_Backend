using Microsoft.EntityFrameworkCore;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Domain.Auth;
using Sanaclub.Domain.Catalogs;
using Sanaclub.Domain.Consents;
using Sanaclub.Domain.EvolutionSheets;
using Sanaclub.Domain.Documents;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;

namespace Sanaclub.Infrastructure.Persistence;

public sealed class SanaclubDbContext : DbContext, ISanaclubDbContext
{
    public SanaclubDbContext(DbContextOptions<SanaclubDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<IdentificationType> IdentificationTypes => Set<IdentificationType>();
    public DbSet<Gender> Genders => Set<Gender>();
    public DbSet<CivilStatus> CivilStatuses => Set<CivilStatus>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public DbSet<PatientStatus> PatientStatuses => Set<PatientStatus>();
    public DbSet<ConsentStatus> ConsentStatuses => Set<ConsentStatus>();
    public DbSet<TreatmentStatus> TreatmentStatuses => Set<TreatmentStatus>();
    public DbSet<EvolutionStatus> EvolutionStatuses => Set<EvolutionStatus>();
    public DbSet<EvolutionSheet> EvolutionSheets => Set<EvolutionSheet>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<InformedConsent> InformedConsents => Set<InformedConsent>();
    public DbSet<TreatmentSheet> TreatmentSheets => Set<TreatmentSheet>();
    public DbSet<GeneratedDocument> GeneratedDocuments => Set<GeneratedDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SanaclubDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
