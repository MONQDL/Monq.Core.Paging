using Microsoft.AspNetCore.Http;
using Monq.Core.Paging.Models;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using static Monq.Core.Paging.Constants.Headers;

namespace Monq.Core.Paging.Extensions;

/// <summary>
/// Методы расширения для получения данных постраничной навигации из <see cref="HttpResponseMessage"/>
/// или <see cref="HttpResponse"/>.
/// </summary>
public static class HttpExtensions
{
    /// <summary>
    /// Получить модель постраничного представления <see cref="PagingResponseHeaders" />. из заголовков HTTP-ответа
    /// <see cref="HttpResponse" />.
    /// </summary>
    /// <param name="httpResponse">HTTP-ответ с заголовками постраничной навигации.</param>
    /// <returns>
    /// Модель постраничного представления <see cref="PagingResponseHeaders" />.
    /// </returns>
    public static PagingResponseHeaders GetPagingData(this HttpResponse? httpResponse)
    {
        if (httpResponse?.Headers is null)
            return new PagingResponseHeaders();

        var headers = httpResponse.Headers;

        var totalRecords = headers.TryGetValue(TotalRecords);
        var totalFilteredRecords = headers.TryGetValue(TotalFilteredRecords);
        var perPage = headers.TryGetValue(PerPage);
        var page = headers.TryGetValue(Page);
        var totalPages = headers.TryGetValue(TotalPages);

        var result = new PagingResponseHeaders(totalRecords, totalFilteredRecords, perPage, page, totalPages);
        return result;
    }

    /// <summary>
    /// Получить модель постраничного представления <see cref="PagingResponseHeaders" />. из заголовков HTTP-ответа
    /// <see cref="HttpResponseMessage" />.
    /// </summary>
    /// <param name="httpResponseMessage">HTTP-ответ с заголовками постраничной навигации.</param>
    /// <returns>
    /// Модель постраничного представления <see cref="PagingResponseHeaders" />.
    /// </returns>
    public static PagingResponseHeaders GetPagingData(this HttpResponseMessage? httpResponseMessage)
    {
        if (httpResponseMessage?.Headers is null)
            return new PagingResponseHeaders();

        var headers = httpResponseMessage.Headers;

        var totalRecords = headers.TryGetValue(TotalRecords);
        var totalFilteredRecords = headers.TryGetValue(TotalFilteredRecords);
        var perPage = headers.TryGetValue(PerPage);
        var page = headers.TryGetValue(Page);
        var totalPages = headers.TryGetValue(TotalPages);

        var result = new PagingResponseHeaders(totalRecords, totalFilteredRecords, perPage, page, totalPages);
        return result;
    }

    static int TryGetValue(this HttpHeaders headers, string key)
    {
        if (!headers.TryGetValues(key, out var values))
            return 0;

        var value = values.FirstOrDefault() ?? string.Empty;
        return !int.TryParse(value, out var result) ? 0 : result;
    }

    static int TryGetValue(this IHeaderDictionary headers, string key)
    {
        if (!headers.ContainsKey(key))
            return 0;

        var value = headers[key].FirstOrDefault() ?? string.Empty;
        return !int.TryParse(value, out var result) ? 0 : result;
    }
}
