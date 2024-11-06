using Monq.Core.Paging.Models;
using System.Text;

namespace Monq.Core.Paging.Extensions;

public static class PagingUriExtensions
{
    /// <summary>
    /// Получить строковое представление модели <see cref="PagingModel"/>.
    /// </summary>
    public static string GetUri(this PagingModel? paging, string url)
    {
        var q = url;

        var qb = new StringBuilder();
        if (paging is not null)
        {
            if (paging.Skip != 0)
                qb.Append("&skip=").Append(paging.Skip);
            if (paging.Page != 0)
                qb.Append("&page=").Append(paging.Page);
            if (paging.PerPage != 0)
                qb.Append("&perPage=").Append(paging.PerPage);
            if (paging.SortCol is not null && !string.IsNullOrWhiteSpace(paging.SortCol))
                qb.Append("&sortCol=").Append(paging.SortCol);
            if (paging.SortDir is not null && !string.IsNullOrWhiteSpace(paging.SortDir))
                qb.Append("&sortDir=").Append(paging.SortDir);
            if (paging.Search is not null && !string.IsNullOrWhiteSpace(paging.Search))
                qb.Append("&search=").Append(paging.Search.Replace("+", "%2B"));
        }

        if (qb.Length == 0)
            return q;

        return q.Contains('?') ? q + qb : q + '?' + qb;
    }
}
