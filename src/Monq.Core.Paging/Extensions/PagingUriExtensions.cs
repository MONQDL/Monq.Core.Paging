using Monq.Core.Paging.Models;
using Monq.Core.Paging.Models.DataTable;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Monq.Core.Paging.Extensions
{
    public static class PagingUriExtensions
    {
        /// <summary>
        /// Получить строку.
        /// </summary>
        public static string GetUri(this DataTable paging, string url)
        {
            var pagingModel = paging.GetPagingModel();
            return pagingModel.GetUri(url);
        }

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

        /// <summary>
        /// Преобразование DataTable в модель постраничной навигации.
        /// </summary>
        /// <typeparam name="T">Принимаемая модель представления.</typeparam>
        /// <param name="model">Модель DataTable.</param>
        /// <param name="sortColMatch">Словарь сопоставлений возможных наименований столбцов из DataTable модели на корректные из <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentException">Обнаружено дублирование сопоставления наименования поля из DataTable модели на поле принимаемой view модели.</exception>
        public static PagingModel GetPagingModel<T>(this DataTable model,
            params (string PropName, Expression<Func<T, object>> PropSelector)[]? sortColMatch)
            where T : class
        {
            var pagingModel = model.GetPagingModel();

            if (!string.IsNullOrWhiteSpace(pagingModel.SortCol) && sortColMatch?.Length > 0)
            {
                try
                {
                    var sortColMatchDict = sortColMatch.ToDictionary(k => k.PropName, v => v.PropSelector);
                    if (sortColMatchDict.TryGetValue(pagingModel.SortCol, out var propSelector))
                        pagingModel.SortCol = propSelector.GetFullPropertyName();
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException(
                        "Обнаружено дублирование сопоставления наименования поля из DataTable модели на поле принимаемой view модели.",
                        ex.ParamName);
                }
            }

            return pagingModel;
        }

        /// <summary>
        /// Преобразование DataTable в модель постраничной навигации.
        /// </summary>
        public static PagingModel GetPagingModel(this DataTable model)
        {
            var sortCol = string.Empty;
            var sortDir = string.Empty;

            if (model.Order?.Any() == true && model.Columns?.Any() == true)
            {
                var orderColumn = model.Order[0].Column;
                if (model.Columns.Count > orderColumn)
                    sortCol = model.Columns[orderColumn].Name;

                sortDir = model.Order[0].Dir;
            }

            // REM: Из-за того, что при проверке на номер страницы и поле Skip и поле Page == 0, происходит выборка
            // пустого массива данных. Для такой ситуации при Skip == 0 в запросе DataTables проставляется первая страница.
            if (model.Start == 0)
                model.Page = 1;

            return new PagingModel
            {
                Page = model.Page,
                Skip = model.Start,
                PerPage = model.Length,
                SortCol = sortCol,
                SortDir = sortDir,
                Search = model.Search?.Value,
            };
        }
    }
}