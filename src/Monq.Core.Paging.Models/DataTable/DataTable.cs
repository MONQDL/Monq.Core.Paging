using System;
using System.Collections.Generic;

namespace Monq.Core.Paging.Models.DataTable
{
    /// <summary>
    /// Модель данных, которая реализует сервис постраничной навигации https://datatables.net/
    /// </summary>
    public class DataTable
    {
        /// <summary>
        /// Номер записи, с которой требуется начать выборку данных.
        /// </summary>
        public int Start { get; set; } = 0;

        /// <summary>
        /// Количество записей в выборке.
        /// </summary>
        public int Length { get; set; } = 0;

        /// <summary>
        /// Служебный параметр, предназначенный для защиты запросов. Поле в ответе должно совпадать с полем в запросе.
        /// </summary>
        public int Draw { get; set; } = 0;

        /// <summary>
        /// Номер страницы.
        /// </summary>
        public int Page { get; set; } = 0;

        /// <summary>
        /// Порядок сортировки.
        /// </summary>
        public IList<OrderColumn> Order { get; set; } = Array.Empty<OrderColumn>();

        /// <summary>
        /// Список колонок, которые содержатся в таблице и требуются для определения сортировки.
        /// </summary>
        public IList<DataColumn> Columns { get; set; } = Array.Empty<DataColumn>();

        /// <summary>
        /// Поисковый запрос.
        /// </summary>
        public DataSearch? Search { get; set; }
    }
}
