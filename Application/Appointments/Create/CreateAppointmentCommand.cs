using MediatR;
using Sanaclub.Application.Appointments.Common;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Exceptions;
using Sanaclub.Domain.Appointments;

namespace Sanaclub.Application.Appointments.Create;

public sealed class CreateAppointmentCommand : IRequest<AppointmentResponse>
{
    public Guid PatientId { get; init; }
    public Guid TherapistUserId { get; init; }
    public DateOnly? AppointmentDate { get; init; }
    public TimeOnly? StartTime { get; init; }
    public string? Notes { get; init; }
    public Guid ScheduledByUserId { get; init; }
}

public sealed class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ITreatmentSheetRepository _treatmentSheetRepository;
    private readonly IEvolutionSheetRepository _evolutionSheetRepository;
    private readonly IAuthRepository _authRepository;
    private readonly ICatalogRepository _catalogRepository;

    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IPatientRepository patientRepository,
        ITreatmentSheetRepository treatmentSheetRepository,
        IEvolutionSheetRepository evolutionSheetRepository,
        IAuthRepository authRepository,
        ICatalogRepository catalogRepository)
    {
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _treatmentSheetRepository = treatmentSheetRepository;
        _evolutionSheetRepository = evolutionSheetRepository;
        _authRepository = authRepository;
        _catalogRepository = catalogRepository;
    }

    public async Task<AppointmentResponse> Handle(
        CreateAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var appointmentDate = request.AppointmentDate!.Value;
        var startTime = request.StartTime!.Value;
        var endDateTime = appointmentDate.ToDateTime(startTime)
            .AddMinutes(AppointmentConstants.AppointmentDurationMinutes);

        if (DateOnly.FromDateTime(endDateTime) != appointmentDate)
        {
            throw new ConflictException("La hora de la cita cruza al día siguiente y no es válida.");
        }

        var endTime = TimeOnly.FromDateTime(endDateTime);

        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("El paciente seleccionado no existe.");
        }

        if (!patient.IsActive)
        {
            throw new ConflictException("El paciente seleccionado no está activo.");
        }

        var therapist = await _authRepository.GetUserByIdAsync(request.TherapistUserId, cancellationToken);
        if (therapist is null)
        {
            throw new NotFoundException("La terapeuta seleccionada no existe.");
        }

        if (!therapist.IsActive)
        {
            throw new ConflictException("La terapeuta seleccionada no está activa.");
        }

        var therapistRoles = await _authRepository.GetActiveRoleCodesByUserIdAsync(
            request.TherapistUserId,
            cancellationToken);

        if (!therapistRoles.Any(roleCode =>
                string.Equals(roleCode, AppointmentConstants.TherapistRoleCode, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException("La cita solo puede asignarse a un usuario con rol THERAPIST.");
        }

        var clinicalReference = await AppointmentClinicalReferenceResolver.ResolveAsync(
            request.PatientId,
            _catalogRepository,
            _treatmentSheetRepository,
            _evolutionSheetRepository,
            cancellationToken);

        var scheduledStatusId = await _catalogRepository.GetAppointmentStatusIdByCodeAsync(
            AppointmentConstants.ScheduledStatusCode,
            cancellationToken);

        if (scheduledStatusId is null)
        {
            throw new NotFoundException("El estado inicial de cita SCHEDULED no fue encontrado.");
        }

        var hasConflict = await _appointmentRepository.HasTherapistScheduleConflictAsync(
            request.TherapistUserId,
            appointmentDate,
            startTime,
            endTime,
            excludedAppointmentId: null,
            cancellationToken);

        if (hasConflict)
        {
            throw new ConflictException("El horario seleccionado no está disponible para la terapeuta.");
        }

        var appointment = new Appointment(
            request.PatientId,
            clinicalReference.TreatmentSheetId,
            clinicalReference.ClinicalReferenceType,
            clinicalReference.ClinicalReferenceId,
            request.TherapistUserId,
            appointmentDate,
            startTime,
            endTime,
            scheduledStatusId.Value,
            patient.FullName,
            request.ScheduledByUserId,
            request.Notes);

        appointment.MarkAsCreated(request.ScheduledByUserId);

        await _appointmentRepository.AddAsync(appointment, cancellationToken);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        var response = await _appointmentRepository.GetResponseByIdAsync(appointment.Id, cancellationToken);
        if (response is null)
        {
            throw new NotFoundException("La cita no fue encontrada.");
        }

        return response;
    }

    private static void ValidateRequest(CreateAppointmentCommand request)
    {
        if (request.PatientId == Guid.Empty)
        {
            throw new AppValidationException("patientId", "El paciente es obligatorio.");
        }

        if (request.TherapistUserId == Guid.Empty)
        {
            throw new AppValidationException("therapistUserId", "La terapeuta es obligatoria.");
        }

        if (request.AppointmentDate is null || request.AppointmentDate.Value == default)
        {
            throw new AppValidationException("appointmentDate", "La fecha de la cita es obligatoria.");
        }

        if (request.StartTime is null)
        {
            throw new AppValidationException("startTime", "La hora de inicio de la cita es obligatoria.");
        }

        if (request.ScheduledByUserId == Guid.Empty)
        {
            throw new AppValidationException("scheduledByUserId", "El usuario que agenda la cita es obligatorio.");
        }
    }
}
