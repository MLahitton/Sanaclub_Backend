using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Domain.EvolutionSheets;
using Sanaclub.Domain.Patients;
using Sanaclub.Domain.TreatmentSheets;
using System;

namespace Sanaclub.Infrastructure.Documents;

public sealed class EvolutionSheetPdfGenerator : IEvolutionSheetPdfGenerator
{
    public Task<byte[]> GenerateAsync(
        Patient patient,
        EvolutionSheet evolutionSheet,
        TreatmentSheet treatmentSheet,
        CancellationToken cancellationToken = default)
    {
        if (patient is null)
        {
            throw new ArgumentNullException(nameof(patient));
        }

        if (evolutionSheet is null)
        {
            throw new ArgumentNullException(nameof(evolutionSheet));
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

                    column.Item().Text("FORMATO DE EVOLUCION DE TERAPIA ALTERNATIVA BIOENERGETICA")
                        .FontSize(16)
                        .SemiBold()
                        .AlignCenter();

                    AddPatientSection(column, patient);
                    AddTreatmentSection(column, treatmentSheet);
                    AddEvolutionSection(column, evolutionSheet);
                    AddCompletionSection(column, evolutionSheet);
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
        AddSectionTitle(column, "SECCION PACIENTE");

        column.Item().Text($"Nombre completo: {patient.FullName}");
        column.Item().Text($"Numero de identificacion: {patient.IdentificationNumber}");
        column.Item().Text($"Fecha de nacimiento: {patient.BirthDate?.ToString("yyyy-MM-dd") ?? "No especificada"}");
        column.Item().Text($"Telefono: {patient.PhoneNumber ?? "No especificado"}");
        column.Item().Text($"Direccion: {patient.Address ?? "No especificada"}");
        column.Item().Text($"Ciudad o municipio: {patient.CityOrMunicipality ?? "No especificado"}");
    }

    private static void AddTreatmentSection(ColumnDescriptor column, TreatmentSheet treatmentSheet)
    {
        AddSectionTitle(column, "SECCION TRATAMIENTO RELACIONADO");

        column.Item().Text($"ID de hoja de tratamiento: {treatmentSheet.Id}");
        column.Item().Text($"Numero de tratamiento: {treatmentSheet.TreatmentNumber ?? "No especificado"}");
        column.Item().Text($"Fecha de consulta: {treatmentSheet.ConsultationDate?.ToString("yyyy-MM-dd") ?? "No especificada"}");
        column.Item().Text($"Estado de tratamiento: {treatmentSheet.TreatmentStatusId}");
    }

    private static void AddEvolutionSection(ColumnDescriptor column, EvolutionSheet evolutionSheet)
    {
        AddSectionTitle(column, "SECCION EVOLUCION");

        column.Item().Text($"Numero de terapia: {evolutionSheet.TherapyNumber ?? "No especificado"}");
        column.Item().Text($"Fecha de evolucion: {evolutionSheet.EvolutionDate?.ToString("yyyy-MM-dd") ?? "No especificada"}");
        column.Item().Text($"Hora de entrada: {evolutionSheet.EntryTime?.ToString("HH:mm") ?? "No especificada"}");
        column.Item().Text($"Hora de salida: {evolutionSheet.ExitTime?.ToString("HH:mm") ?? "No especificada"}");
        column.Item().Text($"Encargada: {evolutionSheet.AssignedStaffName ?? "No especificada"}");
        column.Item().Text($"Terapia: {evolutionSheet.TherapyName ?? "No especificada"}");
        column.Item().Text($"Evolucion: {evolutionSheet.EvolutionNotes}");
        column.Item().Text($"Nuevas indicaciones: {evolutionSheet.NewIndications ?? "No especificadas"}");
        column.Item().Text($"Estado de evolucion: {evolutionSheet.EvolutionStatusId}");
    }

    private static void AddCompletionSection(ColumnDescriptor column, EvolutionSheet evolutionSheet)
    {
        AddSectionTitle(column, "SECCION DE COMPLETADO");

        column.Item().Text($"Fecha de completado: {evolutionSheet.CompletedAtUtc?.ToString("yyyy-MM-dd HH:mm") ?? "No especificada"}");
        column.Item().Text($"Usuario que completo: {evolutionSheet.CompletedByUserId?.ToString() ?? "No especificado"}");
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
