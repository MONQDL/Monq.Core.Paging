using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace Monq.Core.Paging.Helpers;

/// <summary>
/// Класс формирует ссылки для списков данных
/// </summary>
public static class PagingHelper
{
    /// <summary>
    /// Метод формирует список ссылок постраничной навигации в списке. Реализует https://tools.ietf.org/html/rfc5988
    /// </summary>
    /// <param name="baseUri">Uri адрес, к которому будет применена постраничная разбивка.</param>
    /// <param name="itemsCount">Количество записей, для которых ведется постраничная разбивка.</param>
    /// <param name="currentPage">Текущая страница.</param>
    /// <param name="perPage">Максимальное количество элементов на одну страницу.</param>
    public static string GetLinks(Uri? baseUri, long itemsCount, int currentPage, int perPage)
    {
        if (baseUri is null)
            throw new ArgumentNullException(nameof(baseUri), $"{nameof(baseUri)} is null.");

        if (currentPage == 0)
            currentPage = 1;
        if (perPage == 0)
            return string.Empty;

        var linksSb = new StringBuilder();

        var lastPage = (int)Math.Ceiling((double)itemsCount / perPage);

        var prevPage = (currentPage - 1);
        var nextPage = (currentPage + 1);

        if (prevPage > 0)
        {
            linksSb.AppendFormat("<{0}>; rel=\"first\",", GetNewUriWithPagingQuery(baseUri, perPage, 1));
            linksSb.AppendFormat("<{0}>; rel=\"prev\"", GetNewUriWithPagingQuery(baseUri, perPage, prevPage));

            if (nextPage <= lastPage)
                linksSb.Append(",");
        }
        if (nextPage <= lastPage)
        {
            linksSb.AppendFormat("<{0}>; rel=\"next\",", GetNewUriWithPagingQuery(baseUri, perPage, nextPage));
            linksSb.AppendFormat("<{0}>; rel=\"last\"", GetNewUriWithPagingQuery(baseUri, perPage, lastPage));
        }
        return linksSb.ToString();
    }

    static Uri GetNewUriWithPagingQuery(Uri baseUri, int perPage, int page)
    {
        var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(baseUri.Query);

        if (queryDictionary.ContainsKey("perPage"))
        {
            queryDictionary["perPage"] = perPage.ToString();
        }
        else
            queryDictionary.Add("perPage", perPage.ToString());

        if (queryDictionary.ContainsKey("page"))
        {
            queryDictionary["page"] = page.ToString();
        }
        else
            queryDictionary.Add("page", page.ToString());

        var newUri = new UriBuilder(baseUri);
        var newQueryStr = QueryString.Create(queryDictionary);
        newUri.Query = newQueryStr.Value;

        return newUri.Uri;
    }
}
