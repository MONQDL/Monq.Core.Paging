using System.Collections.Generic;
using System.Linq;

namespace Monq.Core.Paging.Models
{
    /// <summary>
    /// Модель ответа после прохождения пейджинга.
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого объекта.</typeparam>
    public class ResponseDataTables<T>
    {
        /// <summary>
        /// Служебный параметр, предназначенный для защиты запросов. Поле в ответе должно совпадать с полем в запросе.
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        /// Количество записей всего.
        /// </summary>
        public int RecordsTotal { get; set; }

        /// <summary>
        /// Количество отфильтрованных записей.
        /// </summary>
        public int RecordsFiltered { get; set; }

        /// <summary>
        /// Результат.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Ошибка.
        /// </summary>
        public string? Error { get; set; }
    }

    /// <summary>
    /// Модель ответа после прохождения пейджинга.
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого объекта.</typeparam>
    public class DataTablesResponse<T>
    {
        /// <summary>
        /// Служебный параметр, предназначеный для защиты запросов. Поле в ответе должно совпадать с полем в запросе.
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        /// Количество записей всего.
        /// </summary>
        public int RecordsTotal { get; set; }

        /// <summary>
        /// Количество отфильтрованных записей.
        /// </summary>
        public int RecordsFiltered { get; set; }

        /// <summary>
        /// Результат.
        /// </summary>
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

        /// <summary>
        /// Ошибка.
        /// </summary>
        public string? Error { get; set; }
    }

    /// <summary>
    /// Модель ответа после прохождения пейджинга.
    /// </summary>
    public class DataTablesResponse
    {
        /// <summary>
        /// Служебный параметр, предназначеный для защиты запросов. Поле в ответе должно совпадать с полем в запросе.
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        /// Количество записей всего.
        /// </summary>
        public int RecordsTotal { get; set; }

        /// <summary>
        /// Количество отфильтрованных записей.
        /// </summary>
        public int RecordsFiltered { get; set; }

        /// <summary>
        /// Результат.
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// Ошибка.
        /// </summary>
        public string? Error { get; set; }
    }
}
