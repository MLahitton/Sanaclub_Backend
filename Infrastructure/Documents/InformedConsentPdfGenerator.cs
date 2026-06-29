using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Domain.Consents;
using Sanaclub.Domain.Patients;
using System;

namespace Sanaclub.Infrastructure.Documents;

public sealed class InformedConsentPdfGenerator : IInformedConsentPdfGenerator
{
    public Task<byte[]> GenerateAsync(
        InformedConsent consent,
        Patient patient,
        string? documentTypeName,
        string? identificationTypeName,
        CancellationToken cancellationToken = default)
    {
        if (consent is null)
        {
            throw new ArgumentNullException(nameof(consent));
        }

        if (patient is null)
        {
            throw new ArgumentNullException(nameof(patient));
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

                    column.Item().Text("CONSENTIMIENTO INFORMADO")
                        .FontSize(16)
                        .SemiBold()
                        .AlignCenter();

                    AddPatientSection(column, patient, identificationTypeName);
                    AddConsentSection(column, consent, documentTypeName);
                    AddDeclarationSection(column);
                    AddSignatureSection(column, consent);
                });
            });
        });

        return Task.FromResult(document.GeneratePdf());
    }

    private static void ConfigureQuestPdfLicense()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    private static void AddPatientSection(
        ColumnDescriptor column,
        Patient patient,
        string? identificationTypeName)
    {
        AddSectionTitle(column, "SECCION PACIENTE");

        var identification = string.IsNullOrWhiteSpace(identificationTypeName)
            ? patient.IdentificationNumber
            : $"{identificationTypeName} {patient.IdentificationNumber}";

        column.Item().Text($"Nombre completo: {patient.FullName}");
        column.Item().Text($"Tipo y numero de identificacion: {identification}");
        column.Item().Text($"Fecha de nacimiento: {patient.BirthDate?.ToString("yyyy-MM-dd") ?? "No especificada"}");
        column.Item().Text($"Telefono: {patient.PhoneNumber ?? "No especificado"}");
        column.Item().Text($"Direccion: {patient.Address ?? "No especificada"}");
        column.Item().Text($"Ciudad o municipio: {patient.CityOrMunicipality ?? "No especificado"}");
    }

    private static void AddConsentSection(ColumnDescriptor column, InformedConsent consent, string? documentTypeName)
    {
        AddSectionTitle(column, "SECCION CONSENTIMIENTO");

        column.Item().Text($"Tipo de consentimiento: {documentTypeName ?? consent.DocumentTypeId.ToString()}");
        column.Item().Text("Estado del consentimiento: Firmado");
        column.Item().Text($"Fecha de creacion: {consent.CreatedAtUtc.ToString("yyyy-MM-dd HH:mm")}" );
        column.Item().Text($"Fecha de firma: {consent.SignedAtUtc?.ToString("yyyy-MM-dd HH:mm") ?? "No especificada"}");
        column.Item().Text($"Usuario que firmo o registro la firma: {consent.SignedByUserId?.ToString() ?? "No especificado"}");
        column.Item().Text($"Texto del consentimiento: {consent.Description ?? "No especificado"}");
        column.Item().Text($"Observaciones o notas: {consent.Notes ?? "No especificado"}");
    }

    private static void AddDeclarationSection(ColumnDescriptor column)
    {
        AddSectionTitle(column, "SECCION DECLARACION");

        column.Item().Text(
            "El paciente o responsable declara haber recibido información suficiente sobre el procedimiento, " +
            "sus beneficios, riesgos, alternativas y condiciones generales, y manifiesta su consentimiento " +
            "conforme a la información registrada en el sistema.");
    }

    private static void AddSignatureSection(ColumnDescriptor column, InformedConsent consent)
    {
        AddSectionTitle(column, "SECCION FIRMA");

        column.Item().Text($"Nombre del paciente o responsable: {consent.PatientSignerName ?? "No especificado"}");
        column.Item().Text($"Fecha de firma: {consent.SignedAtUtc?.ToString("yyyy-MM-dd HH:mm") ?? "No especificada"}");
        column.Item().Text("Firma del paciente/responsable pendiente de integración grafica.");
        column.Item().Text("Firma y RP del profesional pendientes para fase futura.");
    }

    private static void AddSectionTitle(ColumnDescriptor column, string title)
    {
        column.Item().PaddingTop(6);
        column.Item().Text(title).SemiBold();
    }
}
