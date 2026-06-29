using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;
using System;

namespace Sanaclub.Infrastructure.Documents;

public sealed class TreatmentSheetPdfGenerator : ITreatmentSheetPdfGenerator
{
    public Task<byte[]> GenerateAsync(
        Patient patient,
        TreatmentSheet treatmentSheet,
        CancellationToken cancellationToken = default)
    {
        if (patient is null)
        {
            throw new ArgumentNullException(nameof(patient));
        }

        if (treatmentSheet is null)
        {
            throw new ArgumentNullException(nameof(treatmentSheet));
        }

        ConfigureQuestPdfLicense();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.PageColor(Colors.White);

                page.Content().Column(column =>
                {
                    column.Spacing(8);

                    column.Item().Text("INDICACIONES TERAPIA ALTERNATIVA BIOENERGÉTICA")
                        .FontSize(16)
                        .SemiBold()
                        .AlignCenter();

                    AddPatientSection(column, patient);
                    AddInitialDataSection(column, treatmentSheet);
                    AddMedicalIndicationSection(column, treatmentSheet);
                    AddApprovalSection(column, treatmentSheet);
                    AddSignaturePlaceholder(column);
                });
            });
        });

        return Task.FromResult(document.GeneratePdf());
    }

    private static void ConfigureQuestPdfLicense()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    private static void AddPatientSection(ColumnDescriptor column, Patient patient)
    {
        AddSectionTitle(column, "SECCIÓN PACIENTE");

        column.Item().Text($"Nombre completo: {patient.FullName}");
        column.Item().Text($"Número de identificación: {patient.IdentificationNumber}");
        column.Item().Text($"Fecha de nacimiento: {patient.BirthDate?.ToString("yyyy-MM-dd") ?? "No especificada"}");
        column.Item().Text($"Teléfono: {patient.PhoneNumber ?? "No especificado"}");
        column.Item().Text($"Dirección: {patient.Address ?? "No especificada"}");
        column.Item().Text($"Ciudad o municipio: {patient.CityOrMunicipality ?? "No especificado"}");
        column.Item().Text($"Ocupación: {patient.Occupation ?? "No especificada"}");
        column.Item().Text($"Contacto de emergencia: {patient.EmergencyContactName ?? "No especificado"}");
        column.Item().Text($"Relación del contacto: {patient.EmergencyContactRelationship ?? "No especificada"}");
        column.Item().Text($"Teléfono del contacto: {patient.EmergencyContactPhone ?? "No especificado"}");
    }

    private static void AddInitialDataSection(ColumnDescriptor column, TreatmentSheet treatmentSheet)
    {
        AddSectionTitle(column, "SECCIÓN DATOS INICIALES");

        column.Item().Text($"T.No./Número de tratamiento: {treatmentSheet.TreatmentNumber ?? "No especificado"}");
        column.Item().Text($"Fecha de consulta: {treatmentSheet.ConsultationDate?.ToString("yyyy-MM-dd") ?? "No especificada"}");
        column.Item().Text($"Diagnóstico del médico tratante de su E.P.S.: {treatmentSheet.EpsTreatingDoctorDiagnosis ?? "No especificado"}");
        column.Item().Text($"Datos referidos en la historia clínica: {treatmentSheet.ReferredClinicalHistory ?? "No especificado"}");
    }

    private static void AddMedicalIndicationSection(ColumnDescriptor column, TreatmentSheet treatmentSheet)
    {
        AddSectionTitle(column, "SECCIÓN INDICACIÓN MÉDICA");

        column.Item().Text($"Fecha de indicación: {treatmentSheet.IndicationDate?.ToString("yyyy-MM-dd") ?? "No especificada"}");
        column.Item().Text($"Hora de entrada: {treatmentSheet.EntryTime?.ToString("HH:mm") ?? "No especificada"}");
        column.Item().Text($"Hora de salida: {treatmentSheet.ExitTime?.ToString("HH:mm") ?? "No especificada"}");
        column.Item().Text($"Encargada: {treatmentSheet.AssignedStaffName ?? "No especificada"}");
        column.Item().Text($"Terapia: {treatmentSheet.TherapyName ?? "No especificada"}");
        column.Item().Text($"Sistema nervioso / indicaciones: {treatmentSheet.NervousSystemIndications ?? "No especificado"}");
        column.Item().Text($"Descomprimir columna: {(treatmentSheet.DecompressSpine ? "[X]" : "[ ]")}");
        column.Item().Text($"Descomprimir cuello: {(treatmentSheet.DecompressNeck ? "[X]" : "[ ]")}");
        column.Item().Text($"Descomprimir espalda: {(treatmentSheet.DecompressBack ? "[X]" : "[ ]")}");
        column.Item().Text($"Nervios: {(treatmentSheet.EndocrineNerves ? "[X]" : "[ ]")}");
        column.Item().Text($"Defensas: {(treatmentSheet.EndocrineDefenses ? "[X]" : "[ ]")}");
        column.Item().Text($"Hormonas: {(treatmentSheet.EndocrineHormones ? "[X]" : "[ ]")}");
        column.Item().Text($"Reflexología cardiovascular con: {treatmentSheet.CardiovascularReflexologyWith ?? "No especificado"}");
        column.Item().Text($"Reflexología digestiva colon con: {treatmentSheet.DigestiveColonReflexologyWith ?? "No especificado"}");
        column.Item().Text($"Reflexología respiratoria con: {treatmentSheet.RespiratoryReflexologyWith ?? "No especificado"}");
        column.Item().Text($"Reflexología urinaria/excretor con frutas ácidas: {treatmentSheet.UrinaryReflexologyWithAcidFruits ?? "No especificado"}");
        column.Item().Text($"Otros: {treatmentSheet.OtherIndications ?? "No especificado"}");
        column.Item().Text($"Observaciones: {treatmentSheet.Observations ?? "No especificado"}");
    }

    private static void AddApprovalSection(ColumnDescriptor column, TreatmentSheet treatmentSheet)
    {
        AddSectionTitle(column, "SECCIÓN DE APROBACIÓN");

        column.Item().Text($"Fecha de aprobación: {treatmentSheet.ApprovedAtUtc?.ToString("yyyy-MM-dd HH:mm") ?? "No especificada"}");
        column.Item().Text($"Usuario aprobador ID: {treatmentSheet.ApprovedByUserId?.ToString() ?? "No especificado"}");
    }

    private static void AddSignaturePlaceholder(ColumnDescriptor column)
    {
        AddSectionTitle(column, "FIRMA / RP");
        column.Item().Text("Firma y RP pendientes para fase futura.");
    }

    private static void AddSectionTitle(ColumnDescriptor column, string title)
    {
        column.Item().PaddingTop(6);
        column.Item().Text(title).SemiBold();
    }
}
