using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Monq.Core.Paging.Helpers;
using Monq.Core.Paging.Models;
using Monq.Core.Paging.Models.DataTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

namespace Monq.Core.Paging.Extensions
{
    /// <summary>
    /// Расширения для постраничного вывода данных.
    /// </summary>
    public static class PagingExtensions
    {
        struct PagingResult
        {
            public int TotalItemsCount;
            public int ItemsFilteredCount;
            public int CurrentPage;
            public int TotalPages;
        }

        /// <summary>
        /// Получить постраничное представление с мета данными в HTTP Header.
        /// </summary>
        /// <typeparam name="TSource">Тип модели.</typeparam>
        /// <typeparam name="TSortKey">Тип ключа сортировки.</typeparam>
        /// <param name="data">Модель.</param>
        /// <param name="paging">Фильтр постраничной навигации.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="defaultOrder">Сортировка по умолчанию.</param>
        /// <param name="searchExpression">Выражение для поиска.</param>
        /// <param name="link">Ссылка, на базе которой будут сформированы заголовки Http Header Link. Если link == null, то будет использован <paramref name="httpContext" />.</param>
        /// <exception cref="ArgumentNullException">defaultOrder - defaultOrder</exception>
        public static IQueryable<TSource> WithPaging<TSource, TSortKey>(
            this IQueryable<TSource> data,
            PagingModel paging,
            HttpContext httpContext,
            Expression<Func<TSource, TSortKey>>? defaultOrder,
            Expression<Func<TSource, bool>> searchExpression,
            string? link = null) where TSource : class
        {
            if (defaultOrder is null)
                throw new ArgumentNullException(nameof(defaultOrder), $"{nameof(defaultOrder)} is null.");

            return data.GetPaging(paging, httpContext, defaultOrder, link, searchExpression);
        }

        /// <summary>
        /// Получить постраничное представление с мета данными в <see cref="HttpResponse.Headers" />.
        /// </summary>
        /// <typeparam name="TSource">Тип данных.</typeparam>
        /// <typeparam name="TSortKey">Тип ключа сортировки.</typeparam>
        /// <param name="data">Набор данных типа <typeparamref name="TSource" />.</param>
        /// <param name="paging">Фильтр постраничной навигации.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="defaultOrder">Сортировка по умолчанию.</param>
        /// <param name="link">Ссылка, на базе которой будут сформированы заголовки Http Header Link. Если link == null, то будет использован <paramref name="httpContext" />.</param>
        /// <exception cref="ArgumentNullException">defaultOrder - defaultOrder</exception>
        public static IQueryable<TSource> WithPaging<TSource, TSortKey>(
            this IQueryable<TSource> data,
            PagingModel paging,
            HttpContext httpContext,
            Expression<Func<TSource, TSortKey>>? defaultOrder,
            string? link = null) where TSource : class
        {
            if (defaultOrder is null)
                throw new ArgumentNullException(nameof(defaultOrder), $"{nameof(defaultOrder)} is null.");

            return data.GetPaging(paging, httpContext, defaultOrder, link);
        }

        /// <summary>
        /// Получить постраничное представление с мета данными в <see cref="HttpResponse.Headers" />.
        /// </summary>
        /// <typeparam name="TSource">Тип данных.</typeparam>
        /// <typeparam name="TSortKey">Тип ключа сортировки.</typeparam>
        /// <param name="data">Набор данных типа <typeparamref name="TSource" />.</param>
        /// <param name="paging">Фильтр постраничной навигации.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="defaultOrder">Сортировка по умолчанию.</param>
        /// <param name="link">Ссылка, на базе которой будут сформированы заголовки Http Header Link. Если link == null, то будет использован <paramref name="httpContext" />.</param>
        /// <exception cref="ArgumentNullException">defaultOrder - defaultOrder</exception>
        public static IQueryable<TSource> WithPaging<TSource, TSortKey>(
            this IQueryable<TSource> data,
            DataTable paging,
            HttpContext httpContext,
            Expression<Func<TSource, TSortKey>>? defaultOrder,
            string? link = null) where TSource : class
        {
            if (defaultOrder is null)
                throw new ArgumentNullException(nameof(defaultOrder), $"{nameof(defaultOrder)} is null.");

            var searching = new Searching(SearchType.Exclude, paging.Columns?.Where(x => !x.Searchable)?.Select(x => x.Data));
            return data.GetPaging(paging.GetPagingModel(), httpContext, defaultOrder, link, searching);
        }

        /// <summary>
        /// Получить постраничное представление с мета данными в <see cref="HttpResponse.Headers" />.
        /// Используйте этот метод на свой страх и риск, т.к. при операциях skip/take должна быть задана сортировка по умолчанию.
        /// </summary>
        /// <typeparam name="TSource">Тип данных.</typeparam>
        /// <param name="data">Набор данных типа <typeparamref name="TSource" />.</param>
        /// <param name="paging">Фильтр постраничной навигации.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="link">Ссылка, на базе которой будут сформированы заголовки Http Header Link. Если link == null, то будет использован <paramref name="httpContext" />.</param>
        [Obsolete("Используйте этот метод на свой страх и риск, т.к. при операциях skip/take должна быть задана сортировка по умолчанию.", false)]
        public static IQueryable<TSource> WithPaging<TSource>(
            this IQueryable<TSource> data,
            PagingModel paging,
            HttpContext httpContext,
            string? link = null) where TSource : class
        {
            return data.GetPaging<TSource, object>(paging, httpContext, link: link);
        }

        /// <summary>
        /// Получить постраничное представление с мета данными в HTTP Header.
        /// </summary>
        /// <typeparam name="TSource">Тип модели.</typeparam>
        /// <typeparam name="TSortKey">Тип ключа сортировки.</typeparam>
        /// <param name="data">Модель.</param>
        /// <param name="paging">Фильтр постраничной навигации.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="defaultOrder">Сортировка по умолчанию.</param>
        /// <param name="link">Ссылка.</param>
        /// <param name="searchType">Type of the search.</param>
        /// <param name="searchDepth">Глубина поискового запроса. По умолчанию = 1.</param>
        /// <param name="searchProps">Свойства, по которым проводится или не проводится поиск .</param>
        /// <exception cref="ArgumentNullException">defaultOrder - defaultOrder</exception>
        public static IQueryable<TSource> WithPaging<TSource, TSortKey>(
            this IEnumerable<TSource> data,
            PagingModel paging,
            HttpContext httpContext,
            Expression<Func<TSource, TSortKey>>? defaultOrder,
            string? link = null,
            SearchType searchType = SearchType.None,
            int searchDepth = 1,
            params Expression<Func<TSource, object>>[] searchProps
            ) where TSource : class
        {
            if (defaultOrder is null)
            {
                throw new ArgumentNullException(nameof(defaultOrder), $"{nameof(defaultOrder)} is null.");
            }

            return data.AsQueryable().GetPaging(paging, httpContext, defaultOrder, link, Searching.CreateSearching(searchType, searchDepth, searchProps));
        }

        /// <summary>
        /// Получить результат постраничной разбивки в представлении <see cref="DataTablesResponse{TSource}" />.
        /// </summary>
        /// <typeparam name="TSource">Тип модели.</typeparam>
        /// <param name="data">Модель.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        ///   <see cref="DataTablesResponse{TSource}" />
        /// </returns>
        public static DataTablesResponse<TSource> ToDataTablesResponse<TSource>(
            this IQueryable<TSource> data, HttpContext httpContext) where TSource : class
        {
            var pagingData = httpContext.Response.GetPagingData();
            var resultData = data.ToList();
            var result = new DataTablesResponse<TSource>
            {
                RecordsTotal = pagingData.TotalRecords,
                RecordsFiltered = pagingData.TotalFilteredRecords,
                Data = resultData
            };
            return result;
        }

        /// <summary>
        /// Получить результат постраничной разбивки в представлении <see cref="DataTablesResponse{TSource}" />.
        /// </summary>
        /// <typeparam name="TSource">Тип модели.</typeparam>
        /// <param name="data">Модель.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        ///   <see cref="DataTablesResponse{TSource}" />
        /// </returns>
        public static async Task<DataTablesResponse<TSource>> ToDataTablesResponseAsync<TSource>(
            this IQueryable<TSource> data, HttpContext httpContext) where TSource : class
        {
            var pagingData = httpContext.Response.GetPagingData();
            var resultData = await data.ToListAsync();

            var result = new DataTablesResponse<TSource>
            {
                RecordsTotal = pagingData.TotalRecords,
                RecordsFiltered = pagingData.TotalFilteredRecords,
                Data = resultData
            };
            return result;
        }

        /// <summary>
        /// Получить результат постраничной разбивки в представлении <see cref="DataTablesResponse{TResult}" />.
        /// </summary>
        /// <typeparam name="TSource">Тип модели.</typeparam>
        /// <typeparam name="TResult">Тип возвращаемой модели.</typeparam>
        /// <param name="data">Модель.</param>
        /// <param name="mapper">Мэппер для преобразования <typeparamref name="TSource" /> в <typeparamref name="TResult" />.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        ///   <see cref="DataTablesResponse{TSource}" />
        /// </returns>
        public static DataTablesResponse<TResult> ToDataTablesResponse<TSource, TResult>(
            this IQueryable<TSource> data,
            IMapper mapper,
            HttpContext httpContext) where TSource : class
        {
            var pagingData = httpContext.Response.GetPagingData();
            var resultData = data.ToList();
            var result = new DataTablesResponse<TResult>
            {
                RecordsTotal = pagingData.TotalRecords,
                RecordsFiltered = pagingData.TotalFilteredRecords,
                Data = mapper.Map<IEnumerable<TResult>>(resultData)
            };
            return result;
        }

        /// <summary>
        /// Получить результат постраничной разбивки в представлении <see cref="DataTablesResponse{TResult}" />.
        /// </summary>
        /// <typeparam name="TSource">Тип модели.</typeparam>
        /// <typeparam name="TResult">Тип возвращаемой модели.</typeparam>
        /// <param name="data">Модель.</param>
        /// <param name="mapper">Мэппер для преобразования <typeparamref name="TSource" /> в <typeparamref name="TResult" />.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        ///   <see cref="DataTablesResponse{T}" />
        /// </returns>
        public static async Task<DataTablesResponse<TResult>> ToDataTablesResponseAsync<TSource, TResult>(
            this IQueryable<TSource> data,
            IMapper mapper,
            HttpContext httpContext) where TSource : class
        {
            var pagingData = httpContext.Response.GetPagingData();
            var resultData = await data.ToListAsync();
            var result = new DataTablesResponse<TResult>
            {
                RecordsTotal = pagingData.TotalRecords,
                RecordsFiltered = pagingData.TotalFilteredRecords,
                Data = mapper.Map<IEnumerable<TResult>>(resultData)
            };
            return result;
        }

        /// <summary>
        /// Создать модель DataTablesResponse используя указанный response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="response">The response.</param>
        /// <param name="draw">The draw.</param>
        public static DataTablesResponse<T> CreateDataTablesResponse<T>(this IEnumerable<T> elements, HttpResponseMessage response, in int draw = 0)
        {
            var pagingMetaData = response.GetPagingData();
            return new DataTablesResponse<T>
            {
                Data = elements,
                Draw = draw,
                RecordsTotal = pagingMetaData.TotalRecords,
                RecordsFiltered = pagingMetaData.TotalFilteredRecords
            };
        }

        static IQueryable<TSource> GetPaging<TSource, TSortKey>(
            this IQueryable<TSource> data,
            PagingModel paging,
            HttpContext httpContext,
            Expression<Func<TSource, TSortKey>>? defaultOrder = null,
            string? link = null,
            Searching? searching = null) where TSource : class
        {
            var searchExpr = searching.GetExpressionForSearch<TSource>(paging.Search);
            return GetPaging(data, paging, httpContext, defaultOrder, link, searchExpr);
        }

        static IQueryable<TSource> GetPaging<TSource, TSortKey>(
            this IQueryable<TSource> data,
            PagingModel paging,
            HttpContext httpContext,
            Expression<Func<TSource, TSortKey>>? defaultOrder,
            string? link,
            Expression<Func<TSource, bool>> searchExpression) where TSource : class
        {
            var sortedAndFilteredData = ApplySortSearchAndPageFilter(data, paging, out var pagingResult, defaultOrder, searchExpression);

            var itemsTotalCount = pagingResult.TotalItemsCount;
            var itemsCount = pagingResult.ItemsFilteredCount;
            var currentPage = pagingResult.CurrentPage;
            var totalPages = pagingResult.TotalPages;

            Uri baseUri;
            if (link is null)
            {
                var uriBuilder = new UriBuilder(httpContext.Request.Scheme, httpContext.Request.Host.Host, httpContext.Request.Host.Port ?? 80, httpContext.Request.Path.Value)
                {
                    Query = httpContext.Request.QueryString.HasValue ? Uri.EscapeUriString(httpContext.Request.QueryString.Value) : string.Empty
                };
                baseUri = uriBuilder.Uri;
            }
            else
            {
                baseUri = new Uri(link);
            }
            var links = PagingHelper.GetLinks(baseUri, itemsCount, currentPage, paging.PerPage);

            httpContext.Response.Headers.Add(Constants.Headers.Link, links);
            httpContext.Response.Headers.Add(Constants.Headers.TotalRecords, itemsTotalCount.ToString());
            httpContext.Response.Headers.Add(Constants.Headers.TotalFilteredRecords, itemsCount.ToString());
            httpContext.Response.Headers.Add(Constants.Headers.PerPage, paging.PerPage.ToString());
            httpContext.Response.Headers.Add(Constants.Headers.Page, currentPage.ToString());
            httpContext.Response.Headers.Add(Constants.Headers.TotalPages, totalPages.ToString());

            return sortedAndFilteredData;
        }

        static IQueryable<TSource> ApplySortSearchAndPageFilter<TSource, TSortKey>(
            IQueryable<TSource> data,
            PagingModel paging,
            out PagingResult pagingResult,
            Expression<Func<TSource, TSortKey>>? defaultOrder,
            Expression<Func<TSource, bool>>? searchExpression = null) where TSource : class
        {
            var filteredData = data;
            if (!string.IsNullOrWhiteSpace(paging.Search) && searchExpression is not null)
                filteredData = data.Where(searchExpression);

            IQueryable<TSource> sortedAndFilteredData;
            var sortCol = typeof(TSource).GetValidPropertyName(paging.SortCol);
            if (!string.IsNullOrEmpty(sortCol) && sortCol.Length == paging.SortCol.Trim().Length)
            {
                sortedAndFilteredData = filteredData.OrderByProperty(sortCol, paging.SortDir);
            }
            else if (defaultOrder is not null)
                sortedAndFilteredData = filteredData.OrderBy(defaultOrder);
            else
                sortedAndFilteredData = filteredData;

            var totalItemsCount = data.Count();
            var filteredItemsCount = data == filteredData ? totalItemsCount : sortedAndFilteredData.Count();

            sortedAndFilteredData = sortedAndFilteredData.ApplySkipTakeAndGetPagination(paging, filteredItemsCount, out pagingResult);
            pagingResult.TotalItemsCount = totalItemsCount;
            pagingResult.ItemsFilteredCount = filteredItemsCount;
            return sortedAndFilteredData;
        }

        static IQueryable<TSource> ApplySkipTakeAndGetPagination<TSource>(
            this IQueryable<TSource> sorteredAndFilteredData,
            PagingModel paging,
            int filteredRecordsCount,
            out PagingResult pagingResult)
        {
            var itemsCount = filteredRecordsCount;
            var currentPage = paging.Page;
            var totalPages = (int)Math.Ceiling((double)itemsCount / paging.PerPage);

            if (itemsCount > 0)
            {
                if (paging.PerPage <= -1)
                {
                    currentPage = 1;
                    totalPages = 1;
                }
                else if (paging.Skip > 0)
                {
                    currentPage = (int)Math.Ceiling((double)paging.Skip / paging.PerPage) + 1;
                    sorteredAndFilteredData = sorteredAndFilteredData
                        .Skip(paging.Skip)
                        .Take(paging.PerPage);
                }
                else if (paging.Page > totalPages || paging.Page <= 0)
                {
                    currentPage = paging.Page;
                    sorteredAndFilteredData = sorteredAndFilteredData.Where(x => false);
                }
                else
                {
                    currentPage = paging.Page;
                    sorteredAndFilteredData = sorteredAndFilteredData
                        .Skip(paging.PerPage * (currentPage - 1))
                        .Take(paging.PerPage);
                }
            }
            pagingResult = new PagingResult
            {
                CurrentPage = currentPage,
                TotalPages = totalPages
            };

            return sorteredAndFilteredData;
        }
    }
}