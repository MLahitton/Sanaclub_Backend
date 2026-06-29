using MediatR;
using Sanaclub.Application.Appointments.Common;
using Sanaclub.Application.Common.Abstractions;
using Sanaclub.Application.Common.Models;

namespace Sanaclub.Application.Appointments.List;

public sealed class ListAppointmentsQuery : IRequest<PaginatedResult<AppointmentResponse>>
{
    public DateOnly? Date { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public Guid? TherapistUserId { get; init; }
    public Guid? PatientId { get; init; }
    public string? ClinicalReferenceType { get; init; }
    public Guid? ClinicalReferenceId { get; init; }
    public string? Status { get; init; }
    public bool IncludeCancelled { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class ListAppointmentsQueryHandler : IRequestHandler<ListAppointmentsQuery, PaginatedResult<AppointmentResponse>>
{
    private const int MaxPageSize = 100;
    private const int MinPageSize = 1;
    private const int MinPageNumber = 1;

    private readonly IAppointmentRepository _appointmentRepository;

    public ListAppointmentsQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<PaginatedResult<AppointmentResponse>> Handle(
        ListAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < MinPageNumber
            ? MinPageNumber
            : request.PageNumber;

        var pageSize = request.PageSize < MinPageSize
            ? MinPageSize
            : request.PageSize;

        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }

        var filters = new AppointmentListFilters
        {
            Date = request.Date,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            TherapistUserId = request.TherapistUserId,
            PatientId = request.PatientId,
            ClinicalReferenceType = string.IsNullOrWhiteSpace(request.ClinicalReferenceType)
                ? null
                : request.ClinicalReferenceType.Trim().ToUpperInvariant(),
            ClinicalReferenceId = request.ClinicalReferenceId,
            Status = string.IsNullOrWhiteSpace(request.Status)
                ? null
                : request.Status.Trim().ToUpperInvariant(),
            IncludeCancelled = request.IncludeCancelled
        };

        var totalCount = await _appointmentRepository.CountAsync(filters, cancellationToken);
        var items = await _appointmentRepository.ListAsync(filters, pageNumber, pageSize, cancellationToken);

        return new PaginatedResult<AppointmentResponse>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }
}
