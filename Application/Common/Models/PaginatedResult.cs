namespace Sanaclub.Application.Common.Models;

public sealed class PaginatedResult<T>
{
    public PaginatedResult(
        IReadOnlyCollection<T> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "El número de página debe ser mayor a cero.");

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "El tamaño de página debe ser mayor a cero.");

        if (totalCount < 0)
            throw new ArgumentOutOfRangeException(nameof(totalCount), "El total de registros no puede ser negativo.");

        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IReadOnlyCollection<T> Items { get; }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;
}