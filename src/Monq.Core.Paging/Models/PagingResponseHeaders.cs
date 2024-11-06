namespace Monq.Core.Paging.Models;

/// <summary>
/// Модель данных постраничной навигации в заголовках http-ответа.
/// </summary>
public class PagingResponseHeaders
{
    /// <summary>
    /// Общее количество записей.
    /// </summary>
    public int TotalRecords { get; }

    /// <summary>
    /// Количество отфильтрованных записей.
    /// </summary>
    public int TotalFilteredRecords { get; }

    /// <summary>
    /// Количество записей на одну страницу.
    /// </summary>
    public int PerPage { get; }

    /// <summary>
    /// Номер отображаемой страницы.
    /// </summary>
    public int Page { get; }

    /// <summary>
    /// Общее количество страниц.
    /// </summary>
    public int TotalPages { get; }

    internal PagingResponseHeaders() { }

    internal PagingResponseHeaders(
        int totalRecords,
        int totalFilteredRecords,
        int perPage,
        int page,
        int totalPages)
    {
        TotalRecords = totalRecords;
        TotalFilteredRecords = totalFilteredRecords;
        PerPage = perPage;
        Page = page;
        TotalPages = totalPages;
    }
}
